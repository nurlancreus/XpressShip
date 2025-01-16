using Microsoft.EntityFrameworkCore;
using System.Reflection;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Entities.Base;
using XpressShip.Infrastructure.Persistence.Configurations;

// add-migration init -OutputDir ./Persistence/Migrations

namespace XpressShip.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.SeedClients();

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(ShipmentConfiguration))!);

            base.OnModelCreating(modelBuilder);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateDateTimesWhileSavingInterceptor();

            return base.SaveChangesAsync(cancellationToken);
        }

        public DbSet<ApiClient> ApiClients { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<ShipmentRate> ShippingRates { get; set; }


        private void UpdateDateTimesWhileSavingInterceptor()
        {
            var changedEntries = ChangeTracker.Entries<BaseEntity>().Where(e => e.State == EntityState.Modified || e.State == EntityState.Added);

            foreach (var entry in changedEntries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}
