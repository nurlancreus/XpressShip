﻿using MediatR;
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

namespace XpressShip.Application.Features.Shipments.Queries.GetById
{
    public class GetShipmentByIdHandler : IQueryHandler<GetShipmentByIdQuery, ShipmentDTO>
    {
        private readonly IApiClientSession _clientSessionService;
        private readonly IShipmentRepository _shipmentRepository;

        public GetShipmentByIdHandler(IApiClientSession clientSessionService, IShipmentRepository shipmentRepository)
        {
            _clientSessionService = clientSessionService;
            _shipmentRepository = shipmentRepository;
        }

        public async Task<Result<ShipmentDTO>> Handle(GetShipmentByIdQuery request, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentRepository.Table
                                .Include(s => s.Rate)
                                .Include(s => s.OriginAddress)
                                .Include(s => s.DestinationAddress)
                                .Include(s => s.ApiClient)
                                    .ThenInclude(c => c!.Address)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (shipment is null) return Result<ShipmentDTO>.Failure(Error.NotFoundError(nameof(shipment)));

            if (shipment.ApiClient is not null)
            {
                var keysResult = _clientSessionService.GetClientApiAndSecretKey();

                if (keysResult.IsSuccess)
                {
                    if (shipment.ApiClient.ApiKey != keysResult.Value.apiKey || shipment.ApiClient.SecretKey != keysResult.Value.secretKey)
                    {
                        Result<ShipmentDTO>.Failure(Error.UnauthorizedError("You are not authorized to get shipment details"));
                    }
                }
            }

            return Result<ShipmentDTO>.Success(new ShipmentDTO(shipment));
        }
    }
}
