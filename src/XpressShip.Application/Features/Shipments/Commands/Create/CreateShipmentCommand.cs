using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Addresses.DTOs;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Application.Responses;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;

namespace XpressShip.Application.Features.Shipments.Commands.Create
{
    public record CreateShipmentCommand : IRequest<ResponseWithData<ShipmentDTO>>
    {
        public string Status { get; set; } = string.Empty;  // Using enum for status
        public string Method { get; set; } = string.Empty;  // Shipping method using enum
        public double Weight { get; set; }  // Weight of the package
        public string Dimensions { get; set; } = string.Empty;  // Package dimensions (e.g., "10x10x10 cm")
        public AddressCommandDTO? Origin { get; set; }  // Origin address
        public AddressCommandDTO Destination { get; set; } = null!;  // Destination address
        public string? Note { get; set; }  // Additional note for the shipment
        public Guid ShipmentRateId { get; set; }  // Foreign key to ShipmentRate
    }
}
