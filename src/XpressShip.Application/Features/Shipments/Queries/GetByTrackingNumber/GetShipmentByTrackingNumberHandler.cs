using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Interfaces.Services.Session;
using XpressShip.Application.Responses;
using XpressShip.Domain.Exceptions;

namespace XpressShip.Application.Features.Shipments.Queries.GetByTrackingNumber
{
    public class GetShipmentByTrackingNumberHandler : IRequestHandler<GetShipmentByTrackingNumberQuery, ResponseWithData<ShipmentDTO>>
    {
        private readonly IClientSessionService _clientSessionService;
        private readonly IShipmentRepository _shipmentRepository;

        public GetShipmentByTrackingNumberHandler(IClientSessionService clientSessionService, IShipmentRepository shipmentRepository)
        {
            _clientSessionService = clientSessionService;
            _shipmentRepository = shipmentRepository;
        }

        public async Task<ResponseWithData<ShipmentDTO>> Handle(GetShipmentByTrackingNumberQuery request, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentRepository.Table
                                .Include(s => s.Rate)
                                .Include(s => s.OriginAddress)
                                .Include(s => s.DestinationAddress)
                                .Include(s => s.ApiClient)
                                    .ThenInclude(c => c.Address)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(s => s.TrackingNumber == request.TrackingNumber, cancellationToken);

            if (shipment is null) throw new ValidationException("Shipment not found.");

            if (shipment.ApiClient is not null)
            {
                var keys = _clientSessionService.GetClientApiAndSecretKey();

                if (keys is (string apiKey, string secretKey))
                {
                    if (shipment.ApiClient.ApiKey != apiKey || shipment.ApiClient.SecretKey != secretKey)
                    {
                        throw new UnauthorizedAccessException("You cannot get this shipment");
                    }
                }
            }

            return new ResponseWithData<ShipmentDTO>
            {
                IsSuccess = true,
                Data = new ShipmentDTO(shipment),
                Message = "Shipment recevied successfully!"
            };
        }
    }
}
