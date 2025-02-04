using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.Rates.DTOs;

namespace XpressShip.Application.Features.Rates.Queries.GetAll
{
    public record GetAllRatesQuery : IQuery<IEnumerable<ShipmentRateDTO>>
    {
    }
}
