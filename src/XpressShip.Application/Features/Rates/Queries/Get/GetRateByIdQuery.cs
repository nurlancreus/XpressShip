using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.Rates.DTOs;

namespace XpressShip.Application.Features.Rates.Queries.Get
{
    public record GetRateByIdQuery : IQuery<ShipmentRateDTO>
    {
        public Guid Id { get; set; }
    }
}
