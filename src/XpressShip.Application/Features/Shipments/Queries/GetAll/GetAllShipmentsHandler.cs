using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Responses;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;
using XpressShip.Domain.Extensions;
using XpressShip.Domain.Validation;

namespace XpressShip.Application.Features.Shipments.Queries.GetAll
{
    public class GetAllShipmentsHandler : IQueryHandler<GetAllShipmentsQuery, IEnumerable<ShipmentDTO>>
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly bool IsAdmin = true;
        public GetAllShipmentsHandler(IShipmentRepository shipmentRepository)
        {
            _shipmentRepository = shipmentRepository;
        }

        public async Task<Result<IEnumerable<ShipmentDTO>>> Handle(GetAllShipmentsQuery request, CancellationToken cancellationToken)
        {
            if (!IsAdmin) return Result<IEnumerable<ShipmentDTO>>.Failure(Error.UnauthorizedError("You are not authorized to get shipment details"));

            var shipments = _shipmentRepository.Table
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
                .AsNoTracking()
                .AsQueryable();

            // Filter by Status
            if (!string.IsNullOrEmpty(request.Status))
            {
                var shipmentStatus = request.Status.EnsureEnumValueDefined<ShipmentStatus>();
                shipments = shipments.Where(s => s.Status == shipmentStatus);
            }

            // Filter by Client ID
            if (request.ClientId.HasValue)
            {
                shipments = shipments.Where(s => s.ApiClientId == request.ClientId);
            }

            // Filter by Origin Location
            if (request.OriginCountry is string originCountry)
            {
                var origins = shipments.Select(s => new { country = (s.OriginAddress ?? s.ApiClient!.Address).City.Country.Name, city = (s.OriginAddress ?? s.ApiClient!.Address).City.Name });

                if (request.OriginCity is string originCity)
                {
                    shipments = shipments.Where(s => origins.Select(o => o.city).Contains(originCity));
                }
                else
                {
                    shipments = shipments.Where(s => origins.Select(o => o.country).Contains(originCountry));
                }
            }

            // Filter by Destination Location
            if (request.DestinationCountry is string destinationCountry)
            {
                var destinations = shipments.Select(s => new { country = s.DestinationAddress.City.Country.Name, city = s.DestinationAddress.City.Name });

                if (request.DestinationCity is string destinationCity)
                {
                    shipments = shipments.Where(s => destinations.Select(d => d.city).Contains(destinationCity));
                }
                else
                {
                    shipments = shipments.Where(s => destinations.Select(d => d.country).Contains(destinationCountry));
                }
            }

            var dtos = await shipments.Select(s => new ShipmentDTO(s)).ToListAsync(cancellationToken);

            return Result<IEnumerable<ShipmentDTO>>.Success(dtos);
        }
    }
}
