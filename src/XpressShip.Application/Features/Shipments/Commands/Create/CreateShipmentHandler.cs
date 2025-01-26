using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Application.Features.ApiClients.DTOs;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Application.Responses;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Entities.Users;
using XpressShip.Domain.Enums;
using XpressShip.Domain.Exceptions;
using XpressShip.Domain.Extensions;
using XpressShip.Domain.Validation;

namespace XpressShip.Application.Features.Shipments.Commands.Create
{
    public class CreateShipmentHandler : ICommandHandler<CreateShipmentCommand, ShipmentDTO>
    {
        private readonly IApiClientSession _apiClientSession;
        private readonly IJwtSession _jwtSession;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAddressValidationService _addressValidationService;
        private readonly ICountryRepository _countryRepository;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IShipmentRateRepository _shipmentRateRepository;
        private readonly IApiClientRepository _apiClientRepository;
        private readonly IGeoInfoService _geoInfoService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateShipmentHandler(
            IApiClientSession apiClientSession,
            IJwtSession jwtSession,
            UserManager<ApplicationUser> userManager,
            IShipmentRepository shipmentRepository,
            IShipmentRateRepository shipmentRateRepository,
            IApiClientRepository apiClientRepository,
            IGeoInfoService geoInfoService,
            IAddressValidationService addressValidationService,
            ICountryRepository countryRepository,
            IUnitOfWork unitOfWork)
        {
            _apiClientSession = apiClientSession;
            _jwtSession = jwtSession;
            _userManager = userManager;
            _shipmentRepository = shipmentRepository;
            _shipmentRateRepository = shipmentRateRepository;
            _apiClientRepository = apiClientRepository;
            _geoInfoService = geoInfoService;
            _addressValidationService = addressValidationService;
            _countryRepository = countryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ShipmentDTO>> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
        {
            ApiClient? client = null;
            Sender? sender = null;

            if (request.InitiatorRole == "apiClient")
            {
                var keysResult = _apiClientSession.GetClientApiAndSecretKey();

                if (!keysResult.IsSuccess) return Result<ShipmentDTO>.Failure(keysResult.Error);

                // Validate API client
                client = await _apiClientRepository.Table
                    .Include(c => c.Address)
                        .ThenInclude(a => a.City)
                            .ThenInclude(c => c.Country)
                    .FirstOrDefaultAsync(a => a.ApiKey == keysResult.Value.apiKey && a.SecretKey == keysResult.Value.secretKey && a.IsActive, cancellationToken);

                if (client is null)
                    return Result<ShipmentDTO>.Failure(Error.NotFoundError(nameof(client)));
            }
            else if (request.InitiatorRole == "sender")
            {
                var senderIdResult = _jwtSession.GetUserId();

                if (!senderIdResult.IsSuccess) return Result<ShipmentDTO>.Failure(senderIdResult.Error);

                // Validate API client
                sender = await _userManager.Users.OfType<Sender>()
                                .Include(s => s.Address)
                                .FirstOrDefaultAsync(s => s.Id == senderIdResult.Value, cancellationToken);

                if (sender is null)
                    return Result<ShipmentDTO>.Failure(Error.NotFoundError(nameof(sender)));
            }
            else return Result<ShipmentDTO>.Failure(Error.BadRequestError());

            // Validate shipment rate
            var shipmentRate = await _shipmentRateRepository.GetByIdAsync(request.ShipmentRateId, true, cancellationToken);

            if (shipmentRate is null)
                return Result<ShipmentDTO>.Failure(Error.NotFoundError(nameof(shipmentRate)));

            double? originLat = null;
            double? originLon = null;

            Address? originAddress = null;

            if (request.Origin is not null)
            {
                // Validate Origin Address
                var originAddressValidationResult = await _addressValidationService.ValidateCountryCityAndPostalCodeAsync(request.Origin.Country, request.Origin.City, request.Origin.PostalCode, cancellationToken);

                if (!originAddressValidationResult.IsSuccess) return Result<ShipmentDTO>.Failure(originAddressValidationResult.Error);

                var originGeoInfoResult = await _geoInfoService.GetLocationGeoInfoByNameAsync(request.Origin.Country, request.Origin.City, cancellationToken);

                if (!originGeoInfoResult.IsSuccess) return Result<ShipmentDTO>.Failure(originGeoInfoResult.Error);

                originLat = originGeoInfoResult.Value.Latitude;
                originLon = originGeoInfoResult.Value.Longitude;

                originAddress = Address.Create(request.Origin.PostalCode, request.Origin.Street, (double)originLat!, (double)originLon!);

                var originCountry = await _countryRepository.Table
                            .Include(c => c.Cities)
                            .Select(c => new { c.Name, c.Cities })
                            .FirstOrDefaultAsync(c => c.Name == request.Origin.Country, cancellationToken);

                if (originCountry is null) return Result<ShipmentDTO>.Failure(Error.NotFoundError(nameof(originCountry)));

                var originCity = originCountry.Cities.FirstOrDefault(c => c.Name == request.Origin.City);

                if (originCity is null) return Result<ShipmentDTO>.Failure(Error.NotFoundError(nameof(originCity)));

                originAddress.City = originCity;
            }
            else
            {
                originLat = sender?.Address.Latitude ?? client?.Address.Latitude;

                originLon = sender?.Address.Longitude ?? client?.Address.Longitude;

                if (originLat == null || originLon == null) return Result<ShipmentDTO>.Failure(Error.BadRequestError("Origin address's latitude and longitude cannot be null"));
            }

            // Validate Destination Address
            var destAddressValidationResult = await _addressValidationService.ValidateCountryCityAndPostalCodeAsync(request.Destination.Country, request.Destination.City, request.Destination.PostalCode, cancellationToken);

            if (!destAddressValidationResult.IsSuccess) return Result<ShipmentDTO>.Failure(destAddressValidationResult.Error);

            // Calculate and validate distance
            var destinationGeoInfoResult = await _geoInfoService.GetLocationGeoInfoByNameAsync(request.Destination.Country, request.Destination.City, cancellationToken);

            if (!destinationGeoInfoResult.IsSuccess) return Result<ShipmentDTO>.Failure(destinationGeoInfoResult.Error);

            // Validate method
            if (!Enum.TryParse<ShipmentMethod>(request.Method, true, out var method)) return Result<ShipmentDTO>.Failure(Error.BadRequestError("Could not parse the enum"));

            // Create shipment
            var shipment = Shipment.Create(request.Weight, request.Dimensions, method, request.Note);

            shipment.Rate = shipmentRate;

            if (client is not null) shipment.ApiClientId = client.Id;
            else if (sender is not null) shipment.SenderId = sender.Id;

            // Create and validate adresses
            var destinationAddress = Address.Create(request.Destination.PostalCode, request.Destination.Street, destinationGeoInfoResult.Value.Latitude, destinationGeoInfoResult.Value.Longitude);

            var country = await _countryRepository.Table
                            .Include(c => c.Cities)
                            .Select(c => new { c.Name, c.Cities })
                            .FirstOrDefaultAsync(c => c.Name == request.Destination.Country, cancellationToken);

            if (country is null) return Result<ShipmentDTO>.Failure(Error.BadRequestError("Country is not supported"));

            var city = country.Cities.FirstOrDefault(c => c.Name == request.Destination.City);

            if (city is null) return Result<ShipmentDTO>.Failure(Error.BadRequestError("City is not supported"));

            destinationAddress.City = city;

            shipment.DestinationAddress = destinationAddress;

            if (originAddress is not null) shipment.OriginAddress = originAddress;

            // Calculate final cost
            shipment.Cost = shipment.CalculateShippingCost();

            await _shipmentRepository.AddAsync(shipment, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ShipmentDTO>.Success(new ShipmentDTO(shipment));
        }
    }
}
