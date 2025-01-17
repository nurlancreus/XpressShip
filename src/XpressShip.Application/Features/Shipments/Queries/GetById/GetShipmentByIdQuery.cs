using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Application.Responses;

namespace XpressShip.Application.Features.Shipments.Queries.GetById
{
    public record GetShipmentByIdQuery : IRequest<ResponseWithData<ShipmentDTO>>
    {
        public Guid Id {  get; set; }
    }
}
