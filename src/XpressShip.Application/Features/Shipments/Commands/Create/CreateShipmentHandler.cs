using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Application.Interfaces;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Interfaces.Services;
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
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IShipmentRateRepository _shipmentRateRepository;
        private readonly IApiClientRepository _apiClientRepository;
        private readonly ICalculatorService _calculatorService;
        private readonly IGeoInfoService _geoInfoService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateShipmentHandler(
            IClientSessionService clientSessionService,
            IShipmentRepository shipmentRepository,
            IShipmentRateRepository shipmentRateRepository,
            IApiClientRepository apiClientRepository,
            ICalculatorService calculatorService,
            IGeoInfoService geoInfoService,
            IUnitOfWork unitOfWork)
        {
            _clientSessionService = clientSessionService;
            _shipmentRepository = shipmentRepository;
            _shipmentRateRepository = shipmentRateRepository;
            _apiClientRepository = apiClientRepository;
            _calculatorService = calculatorService;
            _geoInfoService = geoInfoService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseWithData<ShipmentDTO>> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
        {
            var (apiKey, secretKey) = _clientSessionService.GetClientApiAndSecretKey();

            // Validate API client
            var client = await _apiClientRepository.Table
                .Include(c => c.Address)
                .FirstOrDefaultAsync(a => a.ApiKey == apiKey && a.SecretKey == secretKey && a.IsActive, cancellationToken);

            if (client is null) throw new ValidationException("Invalid API credentials or inactive client.");


            // Validate shipment rate
            var shipmentRate = await _shipmentRateRepository.GetByIdAsync(request.ShipmentRateId, true, cancellationToken);
            if (shipmentRate is null) throw new ValidationException("Shipment rate not found.");


            // Resolve addresses
            string originCountry = request.Origin?.Country ?? client.Address.Country;
            string originCity = request.Origin?.City ?? client.Address.City;
            string originPostalCode = request.Origin?.PostalCode ?? client.Address.PostalCode;
            string? originState = request.Origin?.State ?? client.Address.State;
            string originStreet = request.Origin?.Street ?? client.Address.Street;

            // Calculate and validate distance
            var originGeoInfo = await _geoInfoService.GetLocationGeoInfoByNameAsync(originCountry, originCity, cancellationToken);
            var destinationGeoInfo = await _geoInfoService.GetLocationGeoInfoByNameAsync(request.Destination.Country, request.Destination.City, cancellationToken);

            // Validate method
            var method = request.Method.EnsureEnumValueDefined<ShipmentMethod>();

            // Create shipment
            var shipment = Shipment.Create(request.Weight, request.Dimensions, method, request.Note);
            shipment.Rate = shipmentRate;
            shipment.ApiClientId = client.Id;

            // Create and validate adresses

            if (request.Origin != null)
            {
                var originAddress = Address.Create(originCountry, originCity, originState, originPostalCode, originStreet, originGeoInfo.Latitude, originGeoInfo.Longitude);
                shipment.OriginAddress = originAddress;
            }

            var destinationAddress = Address.Create(request.Destination.Country, request.Destination.City, request.Destination.State, request.Destination.PostalCode, request.Destination.Street, destinationGeoInfo.Latitude, destinationGeoInfo.Longitude);

            shipment.DestinationAddress = destinationAddress;


            // Calculate final cost
            shipment.Cost = _calculatorService.CalculateShippingCost(shipment);

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
