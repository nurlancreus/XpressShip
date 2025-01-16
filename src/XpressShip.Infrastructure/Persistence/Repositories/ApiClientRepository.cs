using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Domain.Entities;

namespace XpressShip.Infrastructure.Persistence.Repositories
{
    public class ApiClientRepository(AppDbContext dbContext) : Repository<ApiClient>(dbContext), IApiClientRepository
    {
    }
}
