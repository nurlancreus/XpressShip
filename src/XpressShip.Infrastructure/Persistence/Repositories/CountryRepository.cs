using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Domain.Entities;

namespace XpressShip.Infrastructure.Persistence.Repositories
{
    public class CountryRepository(AppDbContext dbContext) : Repository<Country>(dbContext), ICountryRepository
    {
    }
}
