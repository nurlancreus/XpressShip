using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Application.Responses;

namespace XpressShip.Application.Features.Shipments.Queries.GetByTrackingNumber
{
    public record GetShipmentByTrackingNumberQuery : IRequest<ResponseWithData<ShipmentDTO>>
    {
        public string TrackingNumber { get; set; } = string.Empty;
    }
}
