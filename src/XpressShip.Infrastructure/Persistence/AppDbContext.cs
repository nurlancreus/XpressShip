using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Entities.Identity;
using XpressShip.Domain.Entities.Users;
using XpressShip.Infrastructure.Persistence.Configurations;

// add-migration init -OutputDir ./Persistence/Migrations

namespace XpressShip.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.SeedData();

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(ShipmentConfiguration))!);

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));

            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Admin> Admins { get; set; } 
        public DbSet<Sender> Senders { get; set; }
        public DbSet<ApiClient> ApiClients { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<ShipmentRate> ShippingRates { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Payment> Payments { get; set; }

    }
}
