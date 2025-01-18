using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace XpressShip.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiClients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApiKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SecretKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiClients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCodePattern = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaxPercentage = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                    table.CheckConstraint("CK_Country_TaxPercentage", "[TaxPercentage] > 0");
                });

            migrationBuilder.CreateTable(
                name: "ShippingRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaseRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinWeight = table.Column<double>(type: "float", nullable: false),
                    MaxWeight = table.Column<double>(type: "float", nullable: false),
                    MinDistance = table.Column<double>(type: "float", nullable: false),
                    MaxDistance = table.Column<double>(type: "float", nullable: false),
                    MinVolume = table.Column<double>(type: "float", nullable: false),
                    MaxVolume = table.Column<double>(type: "float", nullable: false),
                    BaseRateForKm = table.Column<double>(type: "float", nullable: false),
                    BaseRateForKg = table.Column<double>(type: "float", nullable: false),
                    BaseRateForVolume = table.Column<double>(type: "float", nullable: false),
                    ExpressRateMultiplier = table.Column<double>(type: "float", nullable: false),
                    OvernightRateMultiplier = table.Column<double>(type: "float", nullable: false),
                    ExpressDeliveryTimeMultiplier = table.Column<double>(type: "float", nullable: false),
                    OvernightDeliveryTimeMultiplier = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    CityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_ApiClients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ApiClients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Addresses_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shipments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackingNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstimatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Method = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    Dimensions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    ShipmentRateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DestinationAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApiClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.Id);
                    table.CheckConstraint("CK_Shipment_EstimatedDate", "[EstimatedDate] > getdate()");
                    table.ForeignKey(
                        name: "FK_Shipments_Addresses_DestinationAddressId",
                        column: x => x.DestinationAddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shipments_Addresses_OriginAddressId",
                        column: x => x.OriginAddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Shipments_ApiClients_ApiClientId",
                        column: x => x.ApiClientId,
                        principalTable: "ApiClients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Shipments_ShippingRates_ShipmentRateId",
                        column: x => x.ShipmentRateId,
                        principalTable: "ShippingRates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ApiClients",
                columns: new[] { "Id", "ApiKey", "CompanyName", "CreatedAt", "IsActive", "SecretKey", "UpdatedAt" },
                values: new object[] { new Guid("e1a0b1b0-0001-0000-0000-000000000001"), "kjkcwruVZEgqSAdWdrg3gf3LKyLH2UAzpSZdZxf41RE=", "My Company", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "APe+2/k+jqleimz8LTgYgLsrdqQjKcHtPkbsNwrXYZhape98pO+aTxSvGwbvu9xkYN/EL1SpMyFxADvZ86mpwg==", null });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Code", "CreatedAt", "Name", "PostalCodePattern", "TaxPercentage", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("a1a0b1b0-0001-0000-0000-000000000000"), "RUS", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Russia", "^\\d{6}$", 18m, null },
                    { new Guid("b1a0b1b0-0001-0000-0000-000000000000"), "GEO", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Georgia", "^\\d{4}$", 15m, null },
                    { new Guid("c1a0b1b0-0001-0000-0000-000000000000"), "AZE", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Azerbaijan", "AZ\\s\\d{4}$", 20m, null },
                    { new Guid("d1a0b1b0-0001-0000-0000-000000000000"), "IRN", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Iran", "^\\d{10}$", 25m, null },
                    { new Guid("e1a0b1b0-0001-0000-0000-000000000000"), "TUR", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Turkey", "^\\d{5}$", 18m, null }
                });

            migrationBuilder.InsertData(
                table: "ShippingRates",
                columns: new[] { "Id", "BaseRate", "BaseRateForKg", "BaseRateForKm", "BaseRateForVolume", "CreatedAt", "Description", "ExpressDeliveryTimeMultiplier", "ExpressRateMultiplier", "MaxDistance", "MaxVolume", "MaxWeight", "MinDistance", "MinVolume", "MinWeight", "Name", "OvernightDeliveryTimeMultiplier", "OvernightRateMultiplier", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("f1a0b1b0-0001-0000-0000-000000000001"), 10.00m, 1.5, 0.050000000000000003, 2.0, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rate for small packages within local regions.", 0.80000000000000004, 1.2, 100.0, 0.5, 5.0, 0.0, 0.0, 0.0, "Small Package - Local", 0.5, 1.5, null },
                    { new Guid("f1a0b1b0-0001-0000-0000-000000000002"), 25.00m, 2.0, 0.10000000000000001, 3.0, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rate for medium-sized packages within regional areas.", 0.69999999999999996, 1.3, 500.0, 2.0, 20.0, 101.0, 0.5, 5.0099999999999998, "Medium Package - Regional", 0.40000000000000002, 1.7, null },
                    { new Guid("f1a0b1b0-0001-0000-0000-000000000003"), 50.00m, 3.5, 0.20000000000000001, 4.5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rate for large packages for national deliveries.", 0.59999999999999998, 1.5, 1000.0, 5.0, 50.0, 501.0, 2.0099999999999998, 20.010000000000002, "Large Package - National", 0.29999999999999999, 2.0, null },
                    { new Guid("f1a0b1b0-0001-0000-0000-000000000004"), 75.00m, 5.0, 0.29999999999999999, 6.0, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rate for heavy packages for international deliveries.", 0.5, 1.7, 5000.0, 10.0, 100.0, 1001.0, 5.0099999999999998, 50.009999999999998, "Heavy Package - International", 0.20000000000000001, 2.5, null },
                    { new Guid("f1a0b1b0-0001-0000-0000-000000000005"), 100.00m, 7.0, 0.5, 8.0, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rate for oversized freight shipments globally.", 0.40000000000000002, 2.0, 10000.0, 50.0, 500.0, 5001.0, 10.01, 100.01000000000001, "Oversized Freight - Global", 0.10000000000000001, 3.0, null }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "CountryId", "CreatedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("a1a0b1b0-0001-0000-0000-000000000001"), new Guid("a1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Moscow", null },
                    { new Guid("a1a0b1b0-0001-0000-0000-000000000002"), new Guid("a1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Saint Petersburg", null },
                    { new Guid("a1a0b1b0-0001-0000-0000-000000000003"), new Guid("a1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kazan", null },
                    { new Guid("b1a0b1b0-0001-0000-0000-000000000001"), new Guid("b1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tbilisi", null },
                    { new Guid("b1a0b1b0-0001-0000-0000-000000000002"), new Guid("b1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Batumi", null },
                    { new Guid("c1a0b1b0-0001-0000-0000-000000000001"), new Guid("c1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Baku", null },
                    { new Guid("c1a0b1b0-0001-0000-0000-000000000002"), new Guid("c1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sumqayit", null },
                    { new Guid("c1a0b1b0-0001-0000-0000-000000000003"), new Guid("c1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ganja", null },
                    { new Guid("d1a0b1b0-0001-0000-0000-000000000001"), new Guid("d1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tabriz", null },
                    { new Guid("d1a0b1b0-0001-0000-0000-000000000002"), new Guid("d1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tehran", null },
                    { new Guid("e1a0b1b0-0001-0000-0000-000000000001"), new Guid("e1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ankara", null },
                    { new Guid("e1a0b1b0-0001-0000-0000-000000000002"), new Guid("e1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Istanbul", null },
                    { new Guid("e1a0b1b0-0001-0000-0000-000000000003"), new Guid("e1a0b1b0-0001-0000-0000-000000000000"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Izmir", null }
                });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "Id", "CityId", "ClientId", "CreatedAt", "Latitude", "Longitude", "PostalCode", "Street", "UpdatedAt" },
                values: new object[] { new Guid("e1a0b1b0-0001-0000-0000-000000000002"), new Guid("c1a0b1b0-0001-0000-0000-000000000001"), new Guid("e1a0b1b0-0001-0000-0000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 40.375588499999999, 49.832800900000002, "AZ1000", "Ashiq Molla", null });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CityId",
                table: "Addresses",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ClientId",
                table: "Addresses",
                column: "ClientId",
                unique: true,
                filter: "[ClientId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ApiClients_ApiKey",
                table: "ApiClients",
                column: "ApiKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiClients_SecretKey",
                table: "ApiClients",
                column: "SecretKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId",
                table: "Cities",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ApiClientId",
                table: "Shipments",
                column: "ApiClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_DestinationAddressId",
                table: "Shipments",
                column: "DestinationAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_OriginAddressId",
                table: "Shipments",
                column: "OriginAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ShipmentRateId",
                table: "Shipments",
                column: "ShipmentRateId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_TrackingNumber",
                table: "Shipments",
                column: "TrackingNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRates_Name",
                table: "ShippingRates",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Shipments");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "ShippingRates");

            migrationBuilder.DropTable(
                name: "ApiClients");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
