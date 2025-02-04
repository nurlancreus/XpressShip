using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.Shipments.DTOs;

namespace XpressShip.Application.Features.Shipments.Queries.GetByTrackingNumber
{
    public record GetShipmentByTrackingNumberQuery : IQuery<ShipmentDTO>
    {
        public string TrackingNumber { get; set; } = string.Empty;
    }
}
