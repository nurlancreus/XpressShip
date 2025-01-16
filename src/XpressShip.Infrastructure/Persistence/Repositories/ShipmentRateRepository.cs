using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Domain.Entities;

namespace XpressShip.Infrastructure.Persistence.Repositories
{
    public class ShipmentRateRepository(AppDbContext dbContext) : Repository<ShipmentRate>(dbContext), IShipmentRateRepository
    {
    }
}
