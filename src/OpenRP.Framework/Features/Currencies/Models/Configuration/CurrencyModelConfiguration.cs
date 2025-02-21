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
    public class CurrencyModelConfiguration : IEntityTypeConfiguration<CurrencyModel>
    {
        public void Configure(EntityTypeBuilder<CurrencyModel> builder)
        {
            // Table Name
            builder.ToTable("Currencies");

            // Primary Key
            builder.HasKey(c => c.Id);

            // Property Configurations
            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(100); // Adjust the max length as needed

            // Relationships
            builder.HasMany(c => c.CurrencyUnits)
                   .WithOne(u => u.Currency)
                   .HasForeignKey(u => u.CurrencyId)
                   .OnDelete(DeleteBehavior.Cascade); // Configure delete behavior
        }
    }
}
