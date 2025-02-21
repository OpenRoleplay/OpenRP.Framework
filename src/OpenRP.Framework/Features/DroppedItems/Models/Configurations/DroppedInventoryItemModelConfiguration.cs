using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.DroppedItems.Models.Configurations
{
    public
        class DroppedInventoryItemModelConfiguration : IEntityTypeConfiguration<DroppedInventoryItemModel>
    {
        public void Configure(EntityTypeBuilder<DroppedInventoryItemModel> builder)
        {
            // Map to table "DroppedInventoryItems"
            builder.ToTable("DroppedInventoryItems");

            // Set the primary key.
            builder.HasKey(d => d.Id);

            // Configure required properties.
            builder.Property(d => d.PosX)
                .IsRequired();
            builder.Property(d => d.PosY)
                .IsRequired();
            builder.Property(d => d.PosZ)
                .IsRequired();
            builder.Property(d => d.RotX)
                .IsRequired(false);
            builder.Property(d => d.RotY)
                .IsRequired(false);
            builder.Property(d => d.RotZ)
                .IsRequired(false);

            // Configure relationship:
            // Each DroppedInventoryItemModel has one associated InventoryItemModel.
            // When a DroppedInventoryItemModel is deleted, the associated InventoryItemModel is also deleted.
            builder.HasOne(d => d.InventoryItem)
                   .WithOne(d => d.DroppedInventoryItem)
                   .HasForeignKey<DroppedInventoryItemModel>(d => d.InventoryItemId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
