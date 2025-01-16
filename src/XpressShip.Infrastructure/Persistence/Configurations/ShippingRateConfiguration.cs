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

           
        }
    }
}
