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
    public class VehicleModelSpawnTypeModelModelConfiguration : IEntityTypeConfiguration<VehicleModelSpawnTypeModelModel>
    {
        public void Configure(EntityTypeBuilder<VehicleModelSpawnTypeModelModel> builder)
        {
            // Set primary key and auto-generation if desired.
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();

            // Configure the relationship with VehicleSpawnTypeModel.
            builder.HasOne(x => x.VehicleSpawnType)
                   .WithMany() // Adjust property name if needed.
                   .HasForeignKey(x => x.VehicleSpawnTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure the relationship with VehicleModelModel.
            builder.HasOne(x => x.VehicleModel)
                   .WithMany() // Adjust property name if needed.
                   .HasForeignKey(x => x.VehicleModelId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
