﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using XpressShip.Infrastructure.Persistence;

#nullable disable

namespace XpressShip.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250119103905_init")]
    partial class init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("XpressShip.Domain.Entities.Address", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ClientId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.HasIndex("ClientId")
                        .IsUnique()
                        .HasFilter("[ClientId] IS NOT NULL");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("XpressShip.Domain.Entities.ApiClient", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ApiKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("SecretKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ApiKey")
                        .IsUnique();

                    b.HasIndex("SecretKey")
                        .IsUnique();

                    b.ToTable("ApiClients");
                });

            modelBuilder.Entity("XpressShip.Domain.Entities.City", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CountryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("XpressShip.Domain.Entities.Country", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostalCodePattern")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("TaxPercentage")
                        .HasPrecision(8, 2)
                        .HasColumnType("decimal(8,2)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Countries", t =>
                        {
                            t.HasCheckConstraint("CK_Country_TaxPercentage", "[TaxPercentage] > 0");
                        });
                });

            modelBuilder.Entity("XpressShip.Domain.Entities.Shipment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ApiClientId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Cost")
                        .HasPrecision(12, 2)
                        .HasColumnType("decimal(12,2)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DestinationAddressId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Dimensions")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EstimatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Method")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("OriginAddressId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ShipmentRateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TrackingNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<double>("Weight")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("ApiClientId");

                    b.HasIndex("DestinationAddressId");

                    b.HasIndex("OriginAddressId");

                    b.HasIndex("ShipmentRateId");

                    b.HasIndex("TrackingNumber")
                        .IsUnique();

                    b.ToTable("Shipments", t =>
                        {
                            t.HasCheckConstraint("CK_Shipment_EstimatedDate", "[EstimatedDate] > getdate()");
                        });
                });

            modelBuilder.Entity("XpressShip.Domain.Entities.ShipmentRate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("BaseRate")
                        .HasColumnType("decimal(18,2)");

                    b.Property<double>("BaseRateForKg")
                        .HasColumnType("float");

                    b.Property<double>("BaseRateForKm")
                        .HasColumnType("float");

                    b.Property<double>("BaseRateForVolume")
                        .HasColumnType("float");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("ExpressDeliveryTimeMultiplier")
                        .HasColumnType("float");

                    b.Property<double>("ExpressRateMultiplier")
                        .HasColumnType("float");

                    b.Property<double>("MaxDistance")
                        .HasColumnType("float");

                    b.Property<double>("MaxVolume")
                        .HasColumnType("float");

                    b.Property<double>("MaxWeight")
                        .HasColumnType("float");

                    b.Property<double>("MinDistance")
                        .HasColumnType("float");

                    b.Property<double>("MinVolume")
                        .HasColumnType("float");

                    b.Property<double>("MinWeight")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("OvernightDeliveryTimeMultiplier")
                        .HasColumnType("float");

                    b.Property<double>("OvernightRateMultiplier")
                        .HasColumnType("float");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("ShippingRates");
                });

            modelBuilder.Entity("XpressShip.Domain.Entities.Address", b =>
                {
                    b.HasOne("XpressShip.Domain.Entities.City", "City")
                        .WithMany("Addresses")
                        .HasForeignKey("CityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("XpressShip.Domain.Entities.ApiClient", "Client")
                        .WithOne("Address")
                        .HasForeignKey("XpressShip.Domain.Entities.Address", "ClientId");

                    b.Navigation("City");

                    b.Navigation("Client");
                });

            modelBuilder.Entity("XpressShip.Domain.Entities.City", b =>
                {
                    b.HasOne("XpressShip.Domain.Entities.Country", "Country")
                        .WithMany("Cities")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Country");
                });

            modelBuilder.Entity("XpressShip.Domain.Entities.Shipment", b =>
                {
                    b.HasOne("XpressShip.Domain.Entities.ApiClient", "ApiClient")
                        .WithMany("Shipments")
                        .HasForeignKey("ApiClientId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("XpressShip.Domain.Entities.Address", "DestinationAddress")
                        .WithMany("ShipmentsDestination")
                        .HasForeignKey("DestinationAddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("XpressShip.Domain.Entities.Address", "OriginAddress")
                        .WithMany("ShipmentsOrigin")
                        .HasForeignKey("OriginAddressId");

                    b.HasOne("XpressShip.Domain.Entities.ShipmentRate", "Rate")
                        .WithMany("Shipments")
                        .HasForeignKey("ShipmentRateId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ApiClient");

                    b.Navigation("DestinationAddress");

                    b.Navigation("OriginAddress");

                    b.Navigation("Rate");
                });

            modelBuilder.Entity("XpressShip.Domain.Entities.Address", b =>
                {
                    b.Navigation("ShipmentsDestination");

                    b.Navigation("ShipmentsOrigin");
                });

            modelBuilder.Entity("XpressShip.Domain.Entities.ApiClient", b =>
                {
                    b.Navigation("Address")
                        .IsRequired();

                    b.Navigation("Shipments");
                });

            modelBuilder.Entity("XpressShip.Domain.Entities.City", b =>
                {
                    b.Navigation("Addresses");
                });

            modelBuilder.Entity("XpressShip.Domain.Entities.Country", b =>
                {
                    b.Navigation("Cities");
                });

            modelBuilder.Entity("XpressShip.Domain.Entities.ShipmentRate", b =>
                {
                    b.Navigation("Shipments");
                });
#pragma warning restore 612, 618
        }
    }
}
