using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XpressShip.Domain.Entities;

namespace XpressShip.Infrastructure.Persistence.Configurations
{
    public class ShippingRateConfiguration : IEntityTypeConfiguration<ShipmentRate>
    {
        public void Configure(EntityTypeBuilder<ShipmentRate> builder)
        {
            builder.HasKey(s => s.Id);

            builder.HasMany(s => s.Shipments)
                .WithOne(sh => sh.Rate)
                .HasForeignKey(sh => sh.ShipmentRateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(s => s.Name).IsUnique();
        }
    }
}
