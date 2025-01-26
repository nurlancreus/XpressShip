using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Application.Responses;

namespace XpressShip.Application.Features.Shipments.Queries.GetAll
{
    public record GetAllShipmentsQuery : IQuery<IEnumerable<ShipmentDTO>>
    {
        public string? Status { get; set; }
        public string? OriginCountry {  get; set; }
        public string? OriginCity { get; set; }

        public string? DestinationCountry { get; set; }
        public string? DestinationCity { get; set; }

    }
}
