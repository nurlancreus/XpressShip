using Microsoft.EntityFrameworkCore;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Enums;
using XpressShip.Domain.Extensions;

namespace XpressShip.Application.Features.Shipments.Queries.GetAll
{
    public class GetAllShipmentsHandler : IQueryHandler<GetAllShipmentsQuery, IEnumerable<ShipmentDTO>>
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IJwtSession _jwtSession;
        public GetAllShipmentsHandler(IShipmentRepository shipmentRepository, IJwtSession jwtSession)
        {
            _shipmentRepository = shipmentRepository;
            _jwtSession = jwtSession;
        }

        public async Task<Result<IEnumerable<ShipmentDTO>>> Handle(GetAllShipmentsQuery request, CancellationToken cancellationToken)
        {
            var isAdminResult = _jwtSession.IsAdminAuth();

            if (!isAdminResult.IsSuccess) return Result<IEnumerable<ShipmentDTO>>.Failure(isAdminResult.Error);

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
                .Include(s => s.Sender)
                    .ThenInclude(s => s!.Address)
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

            // Filter by Origin Location
            if (!string.IsNullOrEmpty(request.OriginCountry))
            {
                shipments = shipments.Where(s =>
                    (s.OriginAddress != null && s.OriginAddress.City.Country.Name == request.OriginCountry) ||
                    (s.ApiClient != null && s.ApiClient.Address.City.Country.Name == request.OriginCountry) ||
                    (s.Sender != null && s.Sender.Address.City.Country.Name == request.OriginCountry));

                if (!string.IsNullOrEmpty(request.OriginCity))
                {
                    shipments = shipments.Where(s =>
                        (s.OriginAddress != null && s.OriginAddress.City.Name == request.OriginCity) ||
                        (s.ApiClient != null && s.ApiClient.Address.City.Name == request.OriginCity) ||
                        (s.Sender != null && s.Sender.Address.City.Name == request.OriginCity));
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
