using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Entities;

namespace XpressShip.Infrastructure.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);

            builder
                .HasOne(p => p.Shipment)
                .WithOne(s => s.Payment)
                .HasForeignKey<Payment>(p => p.ShipmentId)
                .IsRequired();

            builder
                .Property(p => p.Status)
                .HasConversion<string>();

            builder
                .Property(p => p.Method)
                .HasConversion<string>();

            builder
                .Property(p => p.Currency)
                .HasConversion<string>();

            builder
                .HasIndex(p => p.TransactionId)
                .IsUnique();
        }
    }
}
