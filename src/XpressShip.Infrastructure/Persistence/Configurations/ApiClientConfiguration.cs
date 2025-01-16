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
    public class ApiClientConfiguration : IEntityTypeConfiguration<ApiClient>
    {
        public void Configure(EntityTypeBuilder<ApiClient> builder)
        {
            builder.HasKey(c => c.Id);

            builder
                .HasIndex(c => c.ApiKey)
                .IsUnique();

            builder
                .HasIndex(c => c.SecretKey)
                .IsUnique();

            builder
                .HasOne(c => c.Address)
                .WithOne(a => a.Client)
                .HasForeignKey<Address>(a => a.ClientId)
                .IsRequired(false);
        }
    }
}
