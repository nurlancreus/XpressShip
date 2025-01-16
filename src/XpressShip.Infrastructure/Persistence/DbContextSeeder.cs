using Microsoft.EntityFrameworkCore;
using XpressShip.Domain.Entities;

namespace XpressShip.Infrastructure.Persistence
{
    public static class DbContextSeeder
    {
        public static ModelBuilder SeedShipmentRates(this ModelBuilder builder)
        {
            var rate1 = ShipmentRate.Create("Small Package - Local", "Rate for small packages within local regions.", 10.00m, 0, 5, 0, 100, 0, 0.5, 0.05, 1.5, 2.0, 1.2, 1.5, 0.8, 0.5);

            rate1.Id = Guid.NewGuid();
            rate1.CreatedAt = DateTime.UtcNow;

            var rate2 = ShipmentRate.Create("Medium Package - Regional", "Rate for medium-sized packages within regional areas.", 25.00m, 5.01, 20, 101, 500, 0.5, 2.0, 0.10, 2.0, 3.0, 1.3, 1.7, 0.7, 0.4);

            rate2.Id = Guid.NewGuid();
            rate2.CreatedAt = DateTime.UtcNow;

            var rate3 = ShipmentRate.Create("Large Package - National", "Rate for large packages for national deliveries.", 50.00m, 20.01, 50, 501, 1000, 2.01, 5.0, 0.20, 3.5, 4.5, 1.5, 2.0, 0.6, 0.3);

            rate3.Id = Guid.NewGuid();
            rate3.CreatedAt = DateTime.UtcNow;

            var rate4 = ShipmentRate.Create("Heavy Package - International", "Rate for heavy packages for international deliveries.", 75.00m, 50.01, 100, 1001, 5000, 5.01, 10.0, 0.30, 5.0, 6.0, 1.7, 2.5, 0.5, 0.2);

            rate4.Id = Guid.NewGuid();
            rate4.CreatedAt = DateTime.UtcNow;

            var rate5 = ShipmentRate.Create("Oversized Freight - Global", "Rate for oversized freight shipments globally.", 100.00m, 100.01, 500, 5001, 10000, 10.01, 50.0, 0.50, 7.0, 8.0, 2.0, 3.0, 0.4, 0.1);

            rate5.Id = Guid.NewGuid();
            rate5.CreatedAt = DateTime.UtcNow;

            builder.Entity<ShipmentRate>().HasData(rate1, rate2, rate3, rate4, rate5);
            return builder;
        }
        public static ModelBuilder SeedClients(this ModelBuilder builder)
        {
            var client = ApiClient.Create("My Company");

            client.Id = Guid.NewGuid();
            client.CreatedAt = DateTime.UtcNow;

            var address = Address.Create("Azerbaijan", "Baku", null, "AZ1000", "Ashiq Molla", 40.3755885, 49.8328009);

            address.Id = Guid.NewGuid();
            address.CreatedAt = DateTime.UtcNow;
            address.ClientId = client.Id;

            builder.Entity<ApiClient>().HasData(client);
            builder.Entity<Address>().HasData(address);

            return builder;
        }
    }
}
