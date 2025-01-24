using MediatR;
using Microsoft.AspNetCore.Http;
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
using XpressShip.Domain.Enums;
using XpressShip.Domain.Exceptions;
using XpressShip.Domain.Extensions;
using XpressShip.Domain.Validation;

namespace XpressShip.Application.Features.Shipments.Commands.Create
{
    public class CreateShipmentHandler : ICommandHandler<CreateShipmentCommand, ShipmentDTO>
    {
        private readonly IApiClientSessionService _clientSessionService;
        private readonly IAddressValidationService _addressValidationService;
        private readonly ICountryRepository _countryRepository;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IShipmentRateRepository _shipmentRateRepository;
        private readonly IApiClientRepository _apiClientRepository;
        private readonly IGeoInfoService _geoInfoService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateShipmentHandler(
            IApiClientSessionService clientSessionService,
            IShipmentRepository shipmentRepository,
            IShipmentRateRepository shipmentRateRepository,
            IApiClientRepository apiClientRepository,
            IGeoInfoService geoInfoService,
            IAddressValidationService addressValidationService,
            ICountryRepository countryRepository,
            IUnitOfWork unitOfWork)
        {
            _clientSessionService = clientSessionService;
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

            var keysResult = _clientSessionService.GetClientApiAndSecretKey();

            if (keysResult.IsSuccess)
            {
                // Validate API client
                client = await _apiClientRepository.Table
                    .Include(c => c.Address)
                        .ThenInclude(a => a.City)
                            .ThenInclude(c => c.Country)
                    .FirstOrDefaultAsync(a => a.ApiKey == keysResult.Value.apiKey && a.SecretKey == keysResult.Value.secretKey && a.IsActive, cancellationToken);

                if (client is null) 
                    return Result<ShipmentDTO>.Failure(Error.NotFoundError(nameof(client)));

            }
            else
            {
                // If api client is null then authorized sender must be checked

                // in our case, we don't have it YET!
            }

            // Validate shipment rate
            var shipmentRate = await _shipmentRateRepository.GetByIdAsync(request.ShipmentRateId, true, cancellationToken);

            if (shipmentRate is null) 
                return Result<ShipmentDTO>.Failure(Error.NotFoundError(nameof(shipmentRate)));

            double originLat = 0;
            double originLon = 0;

            // Validate Destination Address
            var destAddressValidationResult = await _addressValidationService.ValidateCountryCityAndPostalCodeAsync(request.Destination.Country, request.Destination.City, request.Destination.PostalCode, cancellationToken);

            if (!destAddressValidationResult.IsSuccess) return Result<ShipmentDTO>.Failure(destAddressValidationResult.Error);

            Address? originAddress = null;

            if (request.Origin is not null)
            {
                // Validate Origin Address
               var originAddressValidationResult = await _addressValidationService.ValidateCountryCityAndPostalCodeAsync(request.Origin.Country, request.Origin.City, request.Origin.PostalCode,  cancellationToken);

                if (!originAddressValidationResult.IsSuccess) return Result<ShipmentDTO>.Failure(originAddressValidationResult.Error);

                var originGeoInfoResult = await _geoInfoService.GetLocationGeoInfoByNameAsync(request.Origin.Country, request.Origin.City, cancellationToken);

                if (!originGeoInfoResult.IsSuccess) return Result<ShipmentDTO>.Failure(originGeoInfoResult.Error);

                originLat = originGeoInfoResult.Value.Latitude;
                originLon = originGeoInfoResult.Value.Longitude;

                originAddress = Address.Create(request.Origin.PostalCode, request.Origin.Street, originLat, originLon);

                var originCountry = await _countryRepository.Table
                            .Include(c => c.Cities)
                            .Select(c => new { c.Name, c.Cities })
                            .FirstOrDefaultAsync(c => c.Name == request.Origin.Country, cancellationToken);

                originCountry = originCountry.EnsureNonNull();

                var originCity = originCountry.Cities.FirstOrDefault(c => c.Name == request.Origin.City);

                originCity = originCity.EnsureNonNull();

                originAddress.City = originCity;
            }
            else
            {
                originLat = client!.Address.Latitude;
                originLon = client.Address.Longitude;
            }

            // Calculate and validate distance
            var destinationGeoInfoResult = await _geoInfoService.GetLocationGeoInfoByNameAsync(request.Destination.Country, request.Destination.City, cancellationToken);

            if(!destinationGeoInfoResult.IsSuccess) return Result<ShipmentDTO>.Failure(destinationGeoInfoResult.Error);

            // Validate method
            var method = request.Method.EnsureEnumValueDefined<ShipmentMethod>();

            // Create shipment
            var shipment = Shipment.Create(request.Weight, request.Dimensions, method, request.Note);

            shipment.Rate = shipmentRate;
            shipment.ApiClientId = client!.Id;

            // Create and validate adresses
            var destinationAddress = Address.Create(request.Destination.PostalCode, request.Destination.Street, destinationGeoInfoResult.Value.Latitude, destinationGeoInfoResult.Value.Longitude);

            var country = await _countryRepository.Table
                            .Include(c => c.Cities)
                            .Select(c => new { c.Name, c.Cities })
                            .FirstOrDefaultAsync(c => c.Name == request.Destination.Country, cancellationToken);

            country = country.EnsureNonNull();

            var city = country.Cities.FirstOrDefault(c => c.Name == request.Destination.City);

            city = city.EnsureNonNull();

            destinationAddress.City = city;

            shipment.DestinationAddress = destinationAddress;

            if(originAddress is not null) shipment.OriginAddress = originAddress;
 
            // Calculate final cost
            shipment.Cost = shipment.CalculateShippingCost();

            await _shipmentRepository.AddAsync(shipment, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ShipmentDTO>.Success(new ShipmentDTO(shipment));
        }
    }
}
