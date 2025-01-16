using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Domain.Entities;

namespace XpressShip.Infrastructure.Persistence.Repositories
{
    public class ShipmentRepository(AppDbContext dbContext) : Repository<Shipment>(dbContext), IShipmentRepository
    {
    }
}
