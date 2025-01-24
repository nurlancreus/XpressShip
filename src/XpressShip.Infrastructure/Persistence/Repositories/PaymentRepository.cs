using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Domain.Entities;

namespace XpressShip.Infrastructure.Persistence.Repositories
{
    public class PaymentRepository(AppDbContext dbContext) : Repository<Payment>(dbContext), IPaymentRepository
    {
    }
}
