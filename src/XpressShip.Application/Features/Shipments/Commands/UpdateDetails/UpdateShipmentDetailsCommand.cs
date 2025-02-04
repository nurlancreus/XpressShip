using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.Addresses.DTOs;
using XpressShip.Application.Features.Shipments.DTOs;

namespace XpressShip.Application.Features.Shipments.Commands.UpdateDetails
{
    public record UpdateShipmentDetailsCommand : ICommand<ShipmentDTO>
    {
        public Guid? Id { get; set; } 
        public string? Method { get; set; } 
        public double? Weight { get; set; }  
        public string? Dimensions { get; set; } 
        public AddressCommandDTO? Origin { get; set; }
        public AddressCommandDTO? Destination { get; set; } 
        public string? Note { get; set; } 
        public Guid? ShipmentRateId { get; set; }
    }
}
