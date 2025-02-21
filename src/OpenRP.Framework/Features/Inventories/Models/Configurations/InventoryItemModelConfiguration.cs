using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Inventories.Models.Configurations
{
    public class InventoryItemModelConfiguration : IEntityTypeConfiguration<InventoryItemModel>
    {
        public void Configure(EntityTypeBuilder<InventoryItemModel> builder)
        {
            // Map to table (adjust the table name if needed)
            builder.ToTable("InventoryItems");

            // Configure the primary key.
            builder.HasKey(i => i.Id);

            // Configure scalar properties.
            builder.Property(i => i.Amount)
                   .IsRequired();

            builder.Property(i => i.UsesRemaining)
                   .IsRequired(false);

            builder.Property(i => i.KeepOnDeath)
                   .IsRequired();

            builder.Property(i => i.CanDrop)
                   .IsRequired();

            builder.Property(i => i.CanDestroy)
                   .IsRequired();

            // AdditionalData is optional; store as nvarchar(max) (or adjust as needed).
            builder.Property(i => i.AdditionalData)
                   .HasColumnType("longtext")
                   .IsRequired(false);

            builder.Property(i => i.Weight)
                   .IsRequired(false);

            // Configure the relationship with ItemModel.
            // Assuming an InventoryItemModel must have an associated ItemModel.
            builder.HasOne(ii => ii.Item)
                   .WithMany() // No navigation property on the ItemModel side
                   .HasForeignKey(ii => ii.ItemId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship with InventoryModel.
            // Assuming an InventoryItemModel must belong to an InventoryModel.
            builder.HasOne(i => i.Inventory)
                   .WithMany(i => i.Items) // or .WithMany(inv => inv.Items) if InventoryModel exposes a collection
                   .HasForeignKey(i => i.InventoryId)
                   .HasPrincipalKey(inv => inv.Id) // explicitly set the principal key
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
