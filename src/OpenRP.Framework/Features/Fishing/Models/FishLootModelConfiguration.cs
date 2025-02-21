using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Fishing.Models
{
    public class FishLootModelConfiguration : IEntityTypeConfiguration<FishLootModel>
    {
        public void Configure(EntityTypeBuilder<FishLootModel> builder)
        {
            // Map to table; adjust the name if needed.
            builder.ToTable("FishLoot");

            // Configure the primary key.
            builder.HasKey(f => f.Id);
            builder.Property(f => f.Id)
                   .ValueGeneratedOnAdd();

            // Configure scalar properties.
            builder.Property(f => f.Name)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(f => f.FishLootType)
                   .IsRequired();
            // Optional: .HasConversion<int>() if you want to be explicit.

            builder.Property(f => f.MinWeightInGrams)
                   .IsRequired();

            builder.Property(f => f.MaxWeightInGrams)
                   .IsRequired();

            builder.Property(f => f.OddsToCatch)
                   .IsRequired();

            // Configure the relationship with ItemModel.
            builder.HasOne(f => f.Item)
                   .WithMany() // or .WithMany(i => i.FishLoots) if ItemModel defines a collection navigation
                   .HasForeignKey(f => f.ItemId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
