using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Currencies.Models.Configuration
{
    public class CurrencyUnitModelConfiguration : IEntityTypeConfiguration<CurrencyUnitModel>
    {
        public void Configure(EntityTypeBuilder<CurrencyUnitModel> builder)
        {
            // Table Name
            builder.ToTable("CurrencyUnits");

            // Primary Key
            builder.HasKey(u => u.Id);

            // Property Configurations
            builder.Property(u => u.UnitValue)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)"); // Specify precision and scale

            builder.Property(u => u.CurrencyId)
                   .IsRequired();
        }
    }
}
