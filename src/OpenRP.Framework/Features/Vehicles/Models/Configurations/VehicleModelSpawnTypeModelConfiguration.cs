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
    public class VehicleModelSpawnTypeModelConfiguration : IEntityTypeConfiguration<VehicleModelSpawnTypeModel>
    {
        public void Configure(EntityTypeBuilder<VehicleModelSpawnTypeModel> builder)
        {
            // Set primary key and auto-generation if desired.
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();
        }
    }
}
