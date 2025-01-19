using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace XpressShip.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ApiClients",
                columns: new[] { "Id", "ApiKey", "CompanyName", "CreatedAt", "IsActive", "SecretKey", "UpdatedAt" },
                values: new object[] { new Guid("e1a0b1b0-0001-0000-0000-000000000001"), "DtCjCFdg8F5UwN4qh+jq4x5F0F3NZH7kwlStqeJT1xQ=", "My Company", new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), true, "DDdTq31f3XFZB0Q37bylv9OWXdh4JJMD1D0Q2JdXxnjHcv1U8D0anqmagSVizoKyIWUH53/MftB4BTFD/qvwaw==", null });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Code", "CreatedAt", "Name", "PostalCodePattern", "TaxPercentage", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("a1a0b1b0-0001-0000-0000-000000000000"), "RUS", new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), "Russia", "^\\d{6}$", 18m, null },
                    { new Guid("b1a0b1b0-0001-0000-0000-000000000000"), "GEO", new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), "Georgia", "^\\d{4}$", 15m, null },
                    { new Guid("c1a0b1b0-0001-0000-0000-000000000000"), "AZE", new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), "Azerbaijan", "AZ\\s\\d{4}$", 20m, null },
                    { new Guid("d1a0b1b0-0001-0000-0000-000000000000"), "IRN", new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), "Iran", "^\\d{10}$", 25m, null },
                    { new Guid("e1a0b1b0-0001-0000-0000-000000000000"), "TUR", new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), "Turkey", "^\\d{5}$", 18m, null }
                });

            migrationBuilder.InsertData(
                table: "ShippingRates",
                columns: new[] { "Id", "BaseRate", "BaseRateForKg", "BaseRateForKm", "BaseRateForVolume", "CreatedAt", "Description", "ExpressDeliveryTimeMultiplier", "ExpressRateMultiplier", "MaxDistance", "MaxVolume", "MaxWeight", "MinDistance", "MinVolume", "MinWeight", "Name", "OvernightDeliveryTimeMultiplier", "OvernightRateMultiplier", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("f1a0b1b0-0001-0000-0000-000000000001"), 10.00m, 1.5, 0.050000000000000003, 2.0, new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), "Rate for small packages within local regions.", 0.80000000000000004, 1.2, 100.0, 0.5, 5.0, 0.0, 0.0, 0.0, "Small Package - Local", 0.5, 1.5, null },
                    { new Guid("f1a0b1b0-0001-0000-0000-000000000002"), 25.00m, 2.0, 0.10000000000000001, 3.0, new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), "Rate for medium-sized packages within regional areas.", 0.69999999999999996, 1.3, 500.0, 2.0, 20.0, 101.0, 0.5, 5.0099999999999998, "Medium Package - Regional", 0.40000000000000002, 1.7, null },
                    { new Guid("f1a0b1b0-0001-0000-0000-000000000003"), 50.00m, 3.5, 0.20000000000000001, 4.5, new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), "Rate for large packages for national deliveries.", 0.59999999999999998, 1.5, 1000.0, 5.0, 50.0, 501.0, 2.0099999999999998, 20.010000000000002, "Large Package - National", 0.29999999999999999, 2.0, null },
                    { new Guid("f1a0b1b0-0001-0000-0000-000000000004"), 75.00m, 5.0, 0.29999999999999999, 6.0, new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), "Rate for heavy packages for international deliveries.", 0.5, 1.7, 5000.0, 10.0, 100.0, 1001.0, 5.0099999999999998, 50.009999999999998, "Heavy Package - International", 0.20000000000000001, 2.5, null },
                    { new Guid("f1a0b1b0-0001-0000-0000-000000000005"), 100.00m, 7.0, 0.5, 8.0, new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), "Rate for oversized freight shipments globally.", 0.40000000000000002, 2.0, 10000.0, 50.0, 500.0, 5001.0, 10.01, 100.01000000000001, "Oversized Freight - Global", 0.10000000000000001, 3.0, null }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "CountryId", "CreatedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("a1a0b1b0-0001-0000-0000-000000000001"), new Guid("a1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), "Moscow", null },
                    { new Guid("a1a0b1b0-0001-0000-0000-000000000002"), new Guid("a1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), "Saint Petersburg", null },
                    { new Guid("a1a0b1b0-0001-0000-0000-000000000003"), new Guid("a1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), "Kazan", null },
                    { new Guid("b1a0b1b0-0001-0000-0000-000000000001"), new Guid("b1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), "Tbilisi", null },
                    { new Guid("b1a0b1b0-0001-0000-0000-000000000002"), new Guid("b1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), "Batumi", null },
                    { new Guid("c1a0b1b0-0001-0000-0000-000000000001"), new Guid("c1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), "Baku", null },
                    { new Guid("c1a0b1b0-0001-0000-0000-000000000002"), new Guid("c1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), "Sumqayit", null },
                    { new Guid("c1a0b1b0-0001-0000-0000-000000000003"), new Guid("c1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), "Ganja", null },
                    { new Guid("d1a0b1b0-0001-0000-0000-000000000001"), new Guid("d1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), "Tabriz", null },
                    { new Guid("d1a0b1b0-0001-0000-0000-000000000002"), new Guid("d1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), "Tehran", null },
                    { new Guid("e1a0b1b0-0001-0000-0000-000000000001"), new Guid("e1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), "Ankara", null },
                    { new Guid("e1a0b1b0-0001-0000-0000-000000000002"), new Guid("e1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), "Istanbul", null },
                    { new Guid("e1a0b1b0-0001-0000-0000-000000000003"), new Guid("e1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), "Izmir", null }
                });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "Id", "CityId", "ClientId", "CreatedAt", "Latitude", "Longitude", "PostalCode", "Street", "UpdatedAt" },
                values: new object[] { new Guid("e1a0b1b0-0001-0000-0000-000000000002"), new Guid("c1a0b1b0-0001-0000-0000-000000000001"), new Guid("e1a0b1b0-0001-0000-0000-000000000001"), new DateTime(2025, 1, 19, 10, 58, 23, 908, DateTimeKind.Utc).AddTicks(1152), 40.375588499999999, 49.832800900000002, "AZ1000", "Ashiq Molla", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: new Guid("e1a0b1b0-0001-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("a1a0b1b0-0001-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("a1a0b1b0-0001-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("a1a0b1b0-0001-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("b1a0b1b0-0001-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("b1a0b1b0-0001-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("c1a0b1b0-0001-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("c1a0b1b0-0001-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("d1a0b1b0-0001-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("d1a0b1b0-0001-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("e1a0b1b0-0001-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("e1a0b1b0-0001-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("e1a0b1b0-0001-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "ShippingRates",
                keyColumn: "Id",
                keyValue: new Guid("f1a0b1b0-0001-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "ShippingRates",
                keyColumn: "Id",
                keyValue: new Guid("f1a0b1b0-0001-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "ShippingRates",
                keyColumn: "Id",
                keyValue: new Guid("f1a0b1b0-0001-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "ShippingRates",
                keyColumn: "Id",
                keyValue: new Guid("f1a0b1b0-0001-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "ShippingRates",
                keyColumn: "Id",
                keyValue: new Guid("f1a0b1b0-0001-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "ApiClients",
                keyColumn: "Id",
                keyValue: new Guid("e1a0b1b0-0001-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("c1a0b1b0-0001-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a1a0b1b0-0001-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("b1a0b1b0-0001-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("d1a0b1b0-0001-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("e1a0b1b0-0001-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("c1a0b1b0-0001-0000-0000-000000000000"));
        }
    }
}
