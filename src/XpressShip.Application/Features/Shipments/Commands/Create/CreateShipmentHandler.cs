using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Application.Interfaces;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Interfaces.Services;
using XpressShip.Application.Interfaces.Services.Calculator;
using XpressShip.Application.Interfaces.Services.Session;
using XpressShip.Application.Responses;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;
using XpressShip.Domain.Exceptions;
using XpressShip.Domain.Extensions;
using XpressShip.Domain.Validation;

namespace XpressShip.Application.Features.Shipments.Commands.Create
{
    public class CreateShipmentHandler : IRequestHandler<CreateShipmentCommand, ResponseWithData<ShipmentDTO>>
    {
        private readonly IClientSessionService _clientSessionService;
        private readonly IAddressValidationService _addressValidationService;
        private readonly ICountryRepository _countryRepository;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IShipmentRateRepository _shipmentRateRepository;
        private readonly IApiClientRepository _apiClientRepository;
        private readonly ICostCalculatorService _calculatorService;
        private readonly IGeoInfoService _geoInfoService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateShipmentHandler(
            IClientSessionService clientSessionService,
            IShipmentRepository shipmentRepository,
            IShipmentRateRepository shipmentRateRepository,
            IApiClientRepository apiClientRepository,
            ICostCalculatorService calculatorService,
            IGeoInfoService geoInfoService,
            IAddressValidationService addressValidationService,
            ICountryRepository countryRepository,
            IUnitOfWork unitOfWork)
        {
            _clientSessionService = clientSessionService;
            _shipmentRepository = shipmentRepository;
            _shipmentRateRepository = shipmentRateRepository;
            _apiClientRepository = apiClientRepository;
            _calculatorService = calculatorService;
            _geoInfoService = geoInfoService;
            _addressValidationService = addressValidationService;
            _countryRepository = countryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseWithData<ShipmentDTO>> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
        {
            ApiClient? client = null;

            var keys = _clientSessionService.GetClientApiAndSecretKey();


            if (keys is (string apiKey, string secretKey))
            {
                // Validate API client
                client = await _apiClientRepository.Table
                    .Include(c => c.Address)
                        .ThenInclude(a => a.City)
                            .ThenInclude(c => c.Country)
                    .FirstOrDefaultAsync(a => a.ApiKey == apiKey && a.SecretKey == secretKey && a.IsActive, cancellationToken);

                if (client is null) throw new ValidationException("Invalid API credentials or inactive client.");
            }
            else
            {
                // If api client is null then authorized sender must be checked

                // in our case, we don't have it YET!
            }

            // Validate shipment rate
            var shipmentRate = await _shipmentRateRepository.GetByIdAsync(request.ShipmentRateId, true, cancellationToken);
            if (shipmentRate is null) throw new ValidationException("Shipment rate not found.");

            double originLat = 0;
            double originLon = 0;

            // Validate Destination Address
            await _addressValidationService.ValidateCountryCityAndPostalCodeAsync(request.Destination.Country, request.Destination.City, request.Destination.PostalCode, true, cancellationToken);

            if (request.Origin is not null)
            {
                // Validate Origin Address
                await _addressValidationService.ValidateCountryCityAndPostalCodeAsync(request.Origin.Country, request.Origin.City, request.Origin.PostalCode, true, cancellationToken);

                var originGeoInfo = await _geoInfoService.GetLocationGeoInfoByNameAsync(request.Origin.Country, request.Origin.City, cancellationToken);

                originLat = originGeoInfo.Latitude;
                originLon = originGeoInfo.Longitude;

                var originAddress = Address.Create(request.Origin.PostalCode, request.Origin.Street, originLat, originLon);
            }
            else
            {
                originLat = client!.Address.Latitude;
                originLon = client.Address.Longitude;
            }

            // Calculate and validate distance
            var destinationGeoInfo = await _geoInfoService.GetLocationGeoInfoByNameAsync(request.Destination.Country, request.Destination.City, cancellationToken);

            // Validate method
            var method = request.Method.EnsureEnumValueDefined<ShipmentMethod>();

            // Create shipment
            var shipment = Shipment.Create(request.Weight, request.Dimensions, method, request.Note);
            shipment.Rate = shipmentRate;
            shipment.ApiClientId = client!.Id;

            // Create and validate adresses
            var destinationAddress = Address.Create(request.Destination.PostalCode, request.Destination.Street, destinationGeoInfo.Latitude, destinationGeoInfo.Longitude);

            var country = await _countryRepository.Table
                            .Include(c => c.Cities)
                            .Select(c => new { c.Name, c.Cities })
                            .FirstOrDefaultAsync(c => c.Name == request.Destination.Country, cancellationToken);

            country = country.EnsureNonNull();

            var city = country.Cities.FirstOrDefault(c => c.Name == request.Destination.City);

            city = city.EnsureNonNull();

            destinationAddress.City = city;

            shipment.DestinationAddress = destinationAddress;

            // Calculate final cost
            shipment.Cost = _calculatorService.CalculateShippingCost(shipment);

            // Persist shipment
            await _shipmentRepository.AddAsync(shipment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ResponseWithData<ShipmentDTO>
            {
                IsSuccess = true,
                Message = "Shipment created successfully",
                Data = new ShipmentDTO(shipment)
            };
        }
    }
}
