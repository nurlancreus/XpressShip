﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.Addresses.DTOs;
using XpressShip.Application.Features.Shipments.DTOs;

namespace XpressShip.Application.Features.Shipments.Commands.Create.BySender
{
    public record CreateShipmentBySenderCommand : ICommand<ShipmentDTO>
    {
        public string Method { get; set; } = string.Empty;
        public double Weight { get; set; }
        public string Dimensions { get; set; } = string.Empty;  //("10x10x10 cm")
        public AddressCommandDTO? Origin { get; set; }
        public AddressCommandDTO Destination { get; set; } = null!;
        public string? Note { get; set; }
        public Guid ShipmentRateId { get; set; }
    }
}
