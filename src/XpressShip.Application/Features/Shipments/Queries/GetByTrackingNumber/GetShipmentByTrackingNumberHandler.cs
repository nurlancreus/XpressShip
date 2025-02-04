﻿using Microsoft.EntityFrameworkCore;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Entities.Users;

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
                                    .ThenInclude(a => a!.City)
                                        .ThenInclude(c => c.Country)
                                .Include(s => s.DestinationAddress)
                                    .ThenInclude(a => a.City)
                                        .ThenInclude(c => c.Country)
                                .Include(s => s.ApiClient)
                                    .ThenInclude(c => c!.Address)
                                        .ThenInclude(a => a.City)
                                            .ThenInclude(c => c.Country)
                                .Include(s => s.Sender)
                                    .ThenInclude(s => s!.Address)
                                        .ThenInclude(a => a.City)
                                            .ThenInclude(c => c.Country)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(s => s.TrackingNumber == request.TrackingNumber, cancellationToken);

            if (shipment is null) return Result<ShipmentDTO>.Failure(Error.NotFoundError("Shipment is not found"));

            var isAdminResult = _jwtSession.IsAdminAuth();

            if (isAdminResult.IsFailure)
            {
                if (shipment.ApiClient is ApiClient apiClient)
                {
                    var clientIdResult = _apiClientSession.GetClientId();

                    if (clientIdResult.IsFailure) Result<ShipmentDTO>.Failure(clientIdResult.Error);

                    if (apiClient.Id != clientIdResult.Value)
                    {
                        return Result<ShipmentDTO>.Failure(Error.UnauthorizedError("You are not authorized to get shipment details"));
                    }
                }
                else if (shipment.Sender is Sender sender)
                {
                    var userIdResult = _jwtSession.GetUserId();

                    if (userIdResult.IsFailure) Result<ShipmentDTO>.Failure(userIdResult.Error);

                    if (sender.Id != userIdResult.Value)
                    {
                        return Result<ShipmentDTO>.Failure(Error.UnauthorizedError("You are not authorized to get shipment details"));
                    }
                }
                else return Result<ShipmentDTO>.Failure(Error.UnauthorizedError("You are not authorized to get shipment details"));
            }

            return Result<ShipmentDTO>.Success(new ShipmentDTO(shipment));
        }
    }
}
