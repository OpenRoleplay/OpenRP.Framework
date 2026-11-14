using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Maps.Entities;

namespace OpenRP.Framework.Features.Maps.Models.Configurations
{
    public class MapModelConfiguration : IEntityTypeConfiguration<MapModel>
    {
        public void Configure(EntityTypeBuilder<MapModel> builder)
        {
            builder.Property(map => map.Name).HasMaxLength(MapLimits.NameMaxLength).IsRequired();

            // Maps are told apart by name, so no two maps may share one.
            builder.HasIndex(map => map.Name).IsUnique();
        }
    }
}
