using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Interfaces.Services.Session;
using XpressShip.Application.Interfaces.Services;
using XpressShip.Application.Interfaces;
using XpressShip.Application.Responses;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;
using XpressShip.Domain.Validation;
using Microsoft.EntityFrameworkCore;
using XpressShip.Domain.Exceptions;
using XpressShip.Application.Features.Addresses.DTOs;
using XpressShip.Application.DTOs;
using XpressShip.Domain.Extensions;

namespace XpressShip.Application.Features.Shipments.Commands.UpdateDetails
{
    public class UpdateShipmentDetailsHandler : IRequestHandler<UpdateShipmentDetailsCommand, ResponseWithData<ShipmentDTO>>
    {
        private readonly IClientSessionService _clientSessionService;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IShipmentRateRepository _shipmentRateRepository;
        private readonly ICalculatorService _calculatorService;
        private readonly IGeoInfoService _geoInfoService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateShipmentDetailsHandler(
            IClientSessionService clientSessionService,
            IShipmentRepository shipmentRepository,
            IShipmentRateRepository shipmentRateRepository,
            ICalculatorService calculatorService,
            IGeoInfoService geoInfoService,
            IUnitOfWork unitOfWork)
        {
            _clientSessionService = clientSessionService;
            _shipmentRepository = shipmentRepository;
            _shipmentRateRepository = shipmentRateRepository;
            _calculatorService = calculatorService;
            _geoInfoService = geoInfoService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseWithData<ShipmentDTO>> Handle(UpdateShipmentDetailsCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentRepository.Table
                                .Include(s => s.Rate)
                                .Include(s => s.OriginAddress)
                                .Include(s => s.DestinationAddress)
                                .Include(s => s.ApiClient)
                                    .ThenInclude(c => c.Address)
                                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (shipment is null) throw new ValidationException("Shipment not found.");

            var (apiKey, secretKey) = _clientSessionService.GetClientApiAndSecretKey();

            if (shipment.ApiClient.ApiKey != apiKey || shipment.ApiClient.SecretKey != secretKey)
            {
                throw new UnauthorizedAccessException("You cannot update this shipment");
            }

            // Validate shipment rate
            var shipmentRate = shipment.Rate;

            if (request.ShipmentRateId is Guid rateId && shipmentRate.Id != rateId)
            {
                shipmentRate = await _shipmentRateRepository.GetByIdAsync(rateId, true, cancellationToken);

                if (shipmentRate is null) throw new ValidationException("Shipment rate not found.");
            }


            if (request.Origin is AddressCommandDTO originAddress)
            {
                var originGeoInfo = await _geoInfoService.GetLocationGeoInfoByNameAsync(originAddress.Country, originAddress.City, cancellationToken);

                shipment.OriginAddress = Address.Create(originAddress.Country, originAddress.City, originAddress.State, originAddress.PostalCode, originAddress.Street, originGeoInfo.Latitude, originGeoInfo.Longitude);
            }

            if (request.Destination is AddressCommandDTO destinationAddress)
            {
                var destinationGeoInfo = await _geoInfoService.GetLocationGeoInfoByNameAsync(destinationAddress.Country, destinationAddress.City, cancellationToken);

                shipment.DestinationAddress = Address.Create(destinationAddress.Country, destinationAddress.City, destinationAddress.State, destinationAddress.PostalCode, destinationAddress.Street, destinationGeoInfo.Latitude, destinationGeoInfo.Longitude);
            }

            if (request.Weight is double weight && shipment.Weight != weight)
            {

                shipment.Weight = weight;
            }

            if (request.Dimensions is string dimensions && shipment.Dimensions != dimensions)
            {

                shipment.Dimensions = dimensions;
            }

            if (request.Method is string method)
            {
                var shipmentMethod = method.EnsureEnumValueDefined<ShipmentMethod>();

                if (shipmentMethod != shipment.Method) shipment.Method = shipmentMethod;
            }

            // Calculate final cost
            shipment.Cost = _calculatorService.CalculateShippingCost(shipment);

            // Persist shipment
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = new ShipmentDTO(shipment);

            return new ResponseWithData<ShipmentDTO>
            {
                IsSuccess = true,
                Message = "Shipment updated successfully",
                Data = dto
            };
        }
    }
}
