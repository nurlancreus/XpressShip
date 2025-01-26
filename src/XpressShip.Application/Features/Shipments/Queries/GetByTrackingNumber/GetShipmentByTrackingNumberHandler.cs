using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Application.Responses;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Exceptions;

namespace XpressShip.Application.Features.Shipments.Queries.GetByTrackingNumber
{
    public class GetShipmentByTrackingNumberHandler : IQueryHandler<GetShipmentByTrackingNumberQuery, ShipmentDTO>
    {
        private readonly IApiClientSession _apiClientSession;
        private readonly IJwtSession _jwtSession;
        private readonly IShipmentRepository _shipmentRepository;

        public GetShipmentByTrackingNumberHandler(IApiClientSession apiClientSession, IJwtSession jwtSession, IShipmentRepository shipmentRepository)
        {
            _apiClientSession = apiClientSession;
            _jwtSession = jwtSession;
            _shipmentRepository = shipmentRepository;
        }

        public async Task<Result<ShipmentDTO>> Handle(GetShipmentByTrackingNumberQuery request, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentRepository.Table
                                .Include(s => s.Rate)
                                .Include(s => s.OriginAddress)
                                .Include(s => s.DestinationAddress)
                                .Include(s => s.ApiClient)
                                    .ThenInclude(c => c!.Address)
                                .Include(s => s.Sender)
                                    .ThenInclude(s => s!.Address)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(s => s.TrackingNumber == request.TrackingNumber, cancellationToken);

            if (shipment is null) return Result<ShipmentDTO>.Failure(Error.NotFoundError(nameof(shipment)));

            if (shipment.ApiClient is not null)
            {
                var keysResult = _apiClientSession.GetClientApiAndSecretKey();

                if (!keysResult.IsSuccess) return Result<ShipmentDTO>.Failure(keysResult.Error);

                if (shipment.ApiClient.ApiKey != keysResult.Value.apiKey || shipment.ApiClient.SecretKey != keysResult.Value.secretKey)
                {
                    Result<ShipmentDTO>.Failure(Error.UnauthorizedError("You are not authorized to get shipment details"));
                }
            }
            else if (shipment.Sender is not null)
            {
                var senderIdResult = _jwtSession.GetUserId();

                if (!senderIdResult.IsSuccess) return Result<ShipmentDTO>.Failure(senderIdResult.Error);

                if (shipment.Sender.Id != senderIdResult.Value)
                {
                    return Result<ShipmentDTO>.Failure(Error.UnauthorizedError("You are not authorized to get shipment details"));
                }
            }

            return Result<ShipmentDTO>.Success(new ShipmentDTO(shipment));
        }
    }
}
