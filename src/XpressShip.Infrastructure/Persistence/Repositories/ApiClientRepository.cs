using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Domain.Entities;

namespace XpressShip.Infrastructure.Persistence.Repositories
{
    public class ApiClientRepository(AppDbContext dbContext) : Repository<ApiClient>(dbContext), IApiClientRepository
    {
    }
}
