using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.Shipments.DTOs;

namespace XpressShip.Application.Features.Shipments.Queries.GetById
{
    public record GetShipmentByIdQuery : IQuery<ShipmentDTO>
    {
        public Guid Id {  get; set; }
    }
}
