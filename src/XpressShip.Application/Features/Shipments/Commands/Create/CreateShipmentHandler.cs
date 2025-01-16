using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Application.Interfaces;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Interfaces.Services;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IShipmentRateRepository _shipmentRateRepository;
        private readonly IApiClientRepository _apiClientRepository;
        private readonly ICalculatorService _calculatorService;
        private readonly IGeoInfoService _geoInfoService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateShipmentHandler(
            IHttpContextAccessor httpContextAccessor,
            IShipmentRepository shipmentRepository,
            IShipmentRateRepository shipmentRateRepository,
            IApiClientRepository apiClientRepository,
            ICalculatorService calculatorService,
            IGeoInfoService geoInfoService,
            IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _shipmentRepository = shipmentRepository;
            _shipmentRateRepository = shipmentRateRepository;
            _apiClientRepository = apiClientRepository;
            _calculatorService = calculatorService;
            _geoInfoService = geoInfoService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseWithData<ShipmentDTO>> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
        {
            var headers = _httpContextAccessor?.HttpContext?.Request.Headers;

            // Extract API Key and Secret Key from request headers
            if ((headers != null && !headers.TryGetValue("X-Api-Key", out var extractedApiKey)) ||
                (headers != null && !headers.TryGetValue("X-Secret-Key", out var extractedSecretKey)))
            {
                throw new ValidationException("API Key or Secret Key is missing.");
            }

            // Validate API client
            var client = await _apiClientRepository.Table
                .Include(c => c.Address)
                .FirstOrDefaultAsync(a => a.ApiKey == extractedApiKey && a.SecretKey == extractedSecretKey && a.IsActive, cancellationToken);

            if (client is null)
            {
                throw new ValidationException("Invalid API credentials or inactive client.");
            }

            // Validate shipment rate
            var shipmentRate = await _shipmentRateRepository.GetByIdAsync(request.ShipmentRateId, true, cancellationToken);
            if (shipmentRate is null)
            {
                throw new ValidationException("Shipment rate not found.");
            }

            // Validate volume and weight
            var volume = ICalculatorService.CalculateVolume(request.Dimensions);
            IValidationService.ValidateVolume(volume, shipmentRate);
            IValidationService.ValidateWeigth(request.Weight, shipmentRate);

            // Resolve addresses
            string originCountry = request.Origin?.Country ?? client.Address.Country;
            string originCity = request.Origin?.City ?? client.Address.City;
            string originPostalCode = request.Origin?.PostalCode ?? client.Address.PostalCode;
            string? originState = request.Origin?.State ?? client.Address.State;
            string originStreet = request.Origin?.Street ?? client.Address.Street;

            string destinationCountry = request.Destination.Country;
            string destinationCity = request.Destination.City;
            string destinationPostalCode = request.Destination.PostalCode;
            string? destinationState = request.Destination.State;
            string destinationStreet = request.Destination.Street;

            IValidationService.ValidateAddress(originCountry, originCity, originPostalCode, originStreet);
            IValidationService.ValidateAddress(destinationCountry, destinationCity, destinationPostalCode, destinationStreet);

            // Calculate distance
            var originGeoInfo = await _geoInfoService.GetLocationGeoInfoByNameAsync(originCountry, originCity);
            var destinationGeoInfo = await _geoInfoService.GetLocationGeoInfoByNameAsync(destinationCountry, destinationCity);

            var distance = ICalculatorService.CalculateDistance(originGeoInfo.Latitude, originGeoInfo.Longitude, destinationGeoInfo.Latitude, destinationGeoInfo.Longitude);

            IValidationService.ValidateDistance(distance, shipmentRate);

            var method = request.Method.EnsureEnumValueDefined<ShipmentMethod>();

            // Create shipment
            var shipment = Shipment.Create(request.Weight, request.Dimensions, method, request.Note);
            shipment.Cost = _calculatorService.CalculateShippingCost(shipment);
            shipment.ShipmentRateId = shipmentRate.Id;
            shipment.ApiClientId = client.Id;

            var originAddress = Address.Create(originCountry, originCity, originState, originPostalCode, originStreet, originGeoInfo.Latitude, originGeoInfo.Longitude);

            shipment.OriginAddress = originAddress;

            var destinationAddress = Address.Create(destinationCountry, destinationCity, destinationState, destinationPostalCode, destinationStreet, destinationGeoInfo.Latitude, destinationGeoInfo.Longitude);

            shipment.DestinationAddress = destinationAddress;

            // Persist shipment
            await _shipmentRepository.AddAsync(shipment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = new ShipmentDTO(shipment);
            return new ResponseWithData<ShipmentDTO>
            {
                IsSuccess = true,
                Message = "Shipment created successfully",
                Data = dto
            };
        }
    }
}
