using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Items.Models.Configurations
{
    public class ItemModelConfiguration : IEntityTypeConfiguration<ItemModel>
    {
        public void Configure(EntityTypeBuilder<ItemModel> builder)
        {
            // Map the model to the "Items" table.
            builder.ToTable("Items");

            // Configure the primary key.
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id)
                   .ValueGeneratedOnAdd();

            // Configure scalar properties.
            builder.Property(i => i.Name)
                   .IsRequired()
                   .HasMaxLength(255); // Adjust length as needed

            builder.Property(i => i.Description)
                   .IsRequired(false)
                   .HasColumnType("longtext");

            builder.Property(i => i.UseType)
                   .IsRequired();

            builder.Property(i => i.UseValue)
                   .IsRequired(false)
                   .HasColumnType("longtext");

            builder.Property(i => i.Weight)
                   .IsRequired();

            builder.Property(i => i.MaxUses)
                   .IsRequired(false);

            builder.Property(i => i.KeepOnDeath)
                   .IsRequired();

            builder.Property(i => i.CanDrop)
                   .IsRequired();

            builder.Property(i => i.CanDestroy)
                   .IsRequired();

            builder.Property(i => i.Stackable)
                   .IsRequired();

            builder.Property(i => i.DropModelId)
                   .IsRequired(false);
        }
    }
}
