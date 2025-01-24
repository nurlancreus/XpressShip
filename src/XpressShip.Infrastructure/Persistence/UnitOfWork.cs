using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;

namespace XpressShip.Infrastructure.Persistence
{
    public class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
    {
        private readonly AppDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Save changes to the database
            return (await _dbContext.SaveChangesAsync(cancellationToken)) > 0;
        }

        public void Dispose() => _dbContext.Dispose();
    }
}
