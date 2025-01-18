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
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder
                .HasKey(c => c.Id);

            builder
                .HasMany(c => c.Cities)
                .WithOne(c => c.Country)
                .HasForeignKey(c => c.CountryId)
                .IsRequired();

            builder
                .Property(c => c.TaxPercentage)
                .HasPrecision(8, 2);

            builder.ToTable(c => c.HasCheckConstraint("CK_Country_TaxPercentage", "[TaxPercentage] > 0"));
        }
    }
}
