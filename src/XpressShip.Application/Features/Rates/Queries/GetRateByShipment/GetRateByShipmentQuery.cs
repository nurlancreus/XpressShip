using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Rates.DTOs;
using XpressShip.Application.Responses;

namespace XpressShip.Application.Features.Rates.Queries.GetRateByShipment
{
    public record GetRateByShipmentQuery : IRequest<ResponseWithData<ShipmentRateDTO>>
    {
        public Guid ShipmentId { get; set; }
    }
}
