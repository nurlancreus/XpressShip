using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Entities.Identity;
using XpressShip.Domain.Entities.Users;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace XpressShip.Infrastructure.Persistence
{
    public static class DbContextSeeder
    {
        private static readonly Guid AzerbaijanId = Guid.Parse("c1a0b1b0-0001-0000-0000-000000000000");
        private static readonly Guid[] AzeCityIds = 
            [Guid.Parse("c1a0b1b0-0001-0000-0000-000000000001"), 
             Guid.Parse("c1a0b1b0-0001-0000-0000-000000000002"), 
             Guid.Parse("c1a0b1b0-0001-0000-0000-000000000003")
            ];

        private static readonly Guid RussiaId = Guid.Parse("a1a0b1b0-0001-0000-0000-000000000000");
        private static readonly Guid[] RusCityIds =
            [Guid.Parse("a1a0b1b0-0001-0000-0000-000000000001"),
             Guid.Parse("a1a0b1b0-0001-0000-0000-000000000002"),
             Guid.Parse("a1a0b1b0-0001-0000-0000-000000000003")
            ];

        private static readonly Guid GeorgiaId = Guid.Parse("b1a0b1b0-0001-0000-0000-000000000000");
        private static readonly Guid[] GeoCityIds =
            [Guid.Parse("b1a0b1b0-0001-0000-0000-000000000001"),
             Guid.Parse("b1a0b1b0-0001-0000-0000-000000000002")
            ];

        private static readonly Guid IranId = Guid.Parse("d1a0b1b0-0001-0000-0000-000000000000");
        private static readonly Guid[] IranCityIds =
            [Guid.Parse("d1a0b1b0-0001-0000-0000-000000000001"),
             Guid.Parse("d1a0b1b0-0001-0000-0000-000000000002")
            ];

        private static readonly Guid TurkiyeId = Guid.Parse("e1a0b1b0-0001-0000-0000-000000000000");
        private static readonly Guid[] TurCityIds =
            [Guid.Parse("e1a0b1b0-0001-0000-0000-000000000001"),
             Guid.Parse("e1a0b1b0-0001-0000-0000-000000000002"),
             Guid.Parse("e1a0b1b0-0001-0000-0000-000000000003")
            ];

        private static readonly DateTime SeededAt = DateTime.UtcNow;

        public static void SeedData(this ModelBuilder builder)
        {

            #region Seed Super Admin
            var superAdminRole = ApplicationRole.Create("SuperAdmin", "Super Admin Role");
            superAdminRole.Id = Guid.NewGuid().ToString();
            superAdminRole.NormalizedName = "SUPERADMIN";

            builder.Entity<ApplicationRole>().HasData(superAdminRole);

            // Seed the admin user
            var passwordHasher = new PasswordHasher<ApplicationUser>();

            var superAdminUser = Admin.Create("Nurlan", "Shukurov", "nurlancreus", "nurlancreus@example.com", "+994513456776");

            superAdminUser.Id = Guid.NewGuid().ToString();
            superAdminUser.NormalizedUserName = "NURLANCREUS";
            superAdminUser.NormalizedEmail = "NURLANCREUS@EXAMPLE.COM";
            superAdminUser.IsActive = true;

            superAdminUser.PasswordHash = passwordHasher.HashPassword(superAdminUser, "qwerty1234");

            builder.Entity<Admin>().HasData(superAdminUser);

            // Seed user role mapping
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    UserId = superAdminUser.Id,
                    RoleId = superAdminRole.Id,
                }
            );
            #endregion;


            #region Seed Locations
            // Azerbaijan
            var azerbaijan = Country.Create("Azerbaijan", "AZE", @"AZ\s\d{4}$", 20);
            azerbaijan.Id = AzerbaijanId;

            azerbaijan.CreatedAt = SeededAt;

            var baku = City.Create("Baku", azerbaijan.Id);
            baku.Id = AzeCityIds[0];
            baku.CreatedAt = SeededAt;

            var sumqayit = City.Create("Sumqayit", azerbaijan.Id);
            sumqayit.Id = AzeCityIds[1];
            sumqayit.CreatedAt = SeededAt;

            var ganja = City.Create("Ganja", azerbaijan.Id);
            ganja.Id = AzeCityIds[2];
            ganja.CreatedAt = SeededAt;

            // Russia
            var russia = Country.Create("Russia", "RUS", @"^\d{6}$", 18);
            russia.Id = RussiaId;
            russia.CreatedAt = SeededAt;

            var moscow = City.Create("Moscow", russia.Id);
            moscow.Id = RusCityIds[0];
            moscow.CreatedAt = SeededAt;

            var saintPetersburg = City.Create("Saint Petersburg", russia.Id);
            saintPetersburg.Id = RusCityIds[1];
            saintPetersburg.CreatedAt = SeededAt;

            var kazan = City.Create("Kazan", russia.Id);
            kazan.Id = RusCityIds[2];
            kazan.CreatedAt = SeededAt;

            // Georgia
            var georgia = Country.Create("Georgia", "GEO", @"^\d{4}$", 15);
            georgia.Id = GeorgiaId;
            georgia.CreatedAt = SeededAt;

            var tbilisi = City.Create("Tbilisi", georgia.Id);
            tbilisi.Id = GeoCityIds[0];
            tbilisi.CreatedAt = SeededAt;

            var batumi = City.Create("Batumi", georgia.Id);
            batumi.Id = GeoCityIds[1];
            batumi.CreatedAt = SeededAt;

            // Iran
            var iran = Country.Create("Iran", "IRN", @"^\d{10}$", 25);
            iran.Id = IranId;
            iran.CreatedAt = SeededAt;

            var tabriz = City.Create("Tabriz", iran.Id);
            tabriz.Id = IranCityIds[0];
            tabriz.CreatedAt = SeededAt;

            var tehran = City.Create("Tehran", iran.Id);
            tehran.Id = IranCityIds[1];
            tehran.CreatedAt = SeededAt;

            // Turkey
            var turkey = Country.Create("Turkey", "TUR", @"^\d{5}$", 18);
            turkey.Id = TurkiyeId;
            turkey.CreatedAt = SeededAt;

            var ankara = City.Create("Ankara", turkey.Id);
            ankara.Id = TurCityIds[0];
            ankara.CreatedAt = SeededAt;

            var istanbul = City.Create("Istanbul", turkey.Id);
            istanbul.Id = TurCityIds[1];
            istanbul.CreatedAt = SeededAt;

            var izmir = City.Create("Izmir", turkey.Id);
            izmir.Id = TurCityIds[2];
            izmir.CreatedAt = SeededAt;

            // Seeding Countries
            builder.Entity<Country>().HasData(azerbaijan, russia, georgia, iran, turkey);

            // Seeding Cities
            builder.Entity<City>().HasData(baku, sumqayit, ganja, moscow, saintPetersburg, kazan, tbilisi, batumi, tabriz, tehran, ankara, istanbul, izmir);
            #endregion

            #region Seed Client
            var client = ApiClient.Create("My Company", "nurlancreus007@gmail.com");

            client.Id = Guid.Parse("e1a0b1b0-0001-0000-0000-000000000001");
            client.CreatedAt = SeededAt;

            var address = Address.Create("AZ1000", "Ashiq Molla", 40.3755885, 49.8328009);

            address.Id = Guid.Parse("e1a0b1b0-0001-0000-0000-000000000002");
            address.CreatedAt = SeededAt;

            address.ClientId = client.Id;
            address.CityId = AzeCityIds[0];

            builder.Entity<ApiClient>().HasData(client);
            builder.Entity<Address>().HasData(address);
            #endregion

            #region Seed Rates
            var rate1 = ShipmentRate.Create("Small Package - Local", "Rate for small packages within local regions.", 10.00m, 0, 5, 0, 100, 0, 0.5, 0.05, 1.5, 2.0, 1.2, 1.5, 0.8, 0.5);

            rate1.Id = Guid.Parse("f1a0b1b0-0001-0000-0000-000000000001");
            rate1.CreatedAt = SeededAt;

            var rate2 = ShipmentRate.Create("Medium Package - Regional", "Rate for medium-sized packages within regional areas.", 25.00m, 5.01, 20, 101, 500, 0.5, 2.0, 0.10, 2.0, 3.0, 1.3, 1.7, 0.7, 0.4);

            rate2.Id = Guid.Parse("f1a0b1b0-0001-0000-0000-000000000002");
            rate2.CreatedAt = SeededAt;

            var rate3 = ShipmentRate.Create("Large Package - National", "Rate for large packages for national deliveries.", 50.00m, 20.01, 50, 501, 1000, 2.01, 5.0, 0.20, 3.5, 4.5, 1.5, 2.0, 0.6, 0.3);

            rate3.Id = Guid.Parse("f1a0b1b0-0001-0000-0000-000000000003");
            rate3.CreatedAt = SeededAt;

            var rate4 = ShipmentRate.Create("Heavy Package - International", "Rate for heavy packages for international deliveries.", 75.00m, 50.01, 100, 1001, 5000, 5.01, 10.0, 0.30, 5.0, 6.0, 1.7, 2.5, 0.5, 0.2);

            rate4.Id = Guid.Parse("f1a0b1b0-0001-0000-0000-000000000004");
            rate4.CreatedAt = SeededAt;

            var rate5 = ShipmentRate.Create("Oversized Freight - Global", "Rate for oversized freight shipments globally.", 100.00m, 100.01, 500, 5001, 10000, 10.01, 50.0, 0.50, 7.0, 8.0, 2.0, 3.0, 0.4, 0.1);

            rate5.Id = Guid.Parse("f1a0b1b0-0001-0000-0000-000000000005");
            rate5.CreatedAt = SeededAt;

            builder.Entity<ShipmentRate>().HasData(rate1, rate2, rate3, rate4, rate5);
            #endregion
        }
    }
}
