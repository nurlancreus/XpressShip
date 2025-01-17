using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Addresses.DTOs;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Application.Responses;

namespace XpressShip.Application.Features.Shipments.Commands.UpdateDetails
{
    public record UpdateShipmentDetailsCommand : IRequest<ResponseWithData<ShipmentDTO>>
    {
        public Guid? Id { get; set; } // Shipment id
        public string? Method { get; set; } // Shipping method using enum
        public double? Weight { get; set; }  // Weight of the package
        public string? Dimensions { get; set; } // Package dimensions (e.g., "10x10x10 cm")
        public AddressCommandDTO? Origin { get; set; }  // Origin address
        public AddressCommandDTO? Destination { get; set; } // Destination address
        public string? Note { get; set; }  // Additional note for the shipment
        public Guid? ShipmentRateId { get; set; }  // Foreign key to ShipmentRate
    }
}
