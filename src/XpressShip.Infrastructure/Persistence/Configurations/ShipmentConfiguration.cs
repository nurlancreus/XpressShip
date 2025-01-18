using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XpressShip.Domain.Entities;

namespace XpressShip.Infrastructure.Persistence.Configurations
{
    public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
            builder.HasKey(s => s.Id);

            builder
                .HasIndex(s => s.TrackingNumber)
                .IsUnique();

            builder
                .Property(s => s.Status)
                .HasConversion<string>();

            builder
               .Property(s => s.Method)
               .HasConversion<string>();

            builder
                .Property(s => s.Cost)
                .HasPrecision(12, 2);

            builder
                .HasOne(s => s.Rate)
                .WithMany(r => r.Shipments)
                .HasForeignKey(s => s.ShipmentRateId);

            builder
                .HasOne(s => s.OriginAddress)
                .WithMany(a => a.ShipmentsOrigin)
                .HasForeignKey(s => s.OriginAddressId)
                .IsRequired(false);

            builder
               .HasOne(s => s.DestinationAddress)
               .WithMany(a => a.ShipmentsDestination)
               .HasForeignKey(s => s.DestinationAddressId);

            builder
                .HasOne(s => s.ApiClient)
                .WithMany(c => c.Shipments)
                .HasForeignKey(s => s.ApiClientId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable(s => s.HasCheckConstraint("CK_Shipment_EstimatedDate", "[EstimatedDate] > getdate()"));
        }
    }
}
