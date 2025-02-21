using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Vehicles.Models.Configurations
{
    public class VehicleModelDefaultColorConfiguration : IEntityTypeConfiguration<VehicleModelDefaultColorModel>
    {
        public void Configure(EntityTypeBuilder<VehicleModelDefaultColorModel> builder)
        {
            // Specify the table name if needed.
            builder.ToTable("VehicleModelDefaultColors");

            // Set the primary key.
            builder.HasKey(x => x.Id);

            // Configure the Id property to be generated on add.
            builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();

            // Configure the VehicleModelId property as required.
            builder.Property(x => x.VehicleModelId)
                   .IsRequired();

            // Configure the Color1 and Color2 properties as required.
            builder.Property(x => x.Color1)
                   .IsRequired();

            builder.Property(x => x.Color2)
                   .IsRequired();

            // Configure the relationship between VehicleModelDefaultColorModel and VehicleModelModel.
            // Assuming that VehicleModelModel has a collection navigation property named DefaultColors.
            builder.HasOne(x => x.VehicleModel)
                   .WithMany()
                   .HasForeignKey(x => x.VehicleModelId)
                   .OnDelete(DeleteBehavior.Cascade); // adjust the delete behavior as needed.
        }
    }
}
