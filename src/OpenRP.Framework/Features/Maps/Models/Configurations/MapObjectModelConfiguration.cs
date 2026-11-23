using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Maps.Entities;

namespace OpenRP.Framework.Features.Maps.Models.Configurations
{
    public class MapObjectModelConfiguration : IEntityTypeConfiguration<MapObjectModel>
    {
        public void Configure(EntityTypeBuilder<MapObjectModel> builder)
        {
            // No database default for the world and interior: EF leaves a 0 out of the insert when there is one, so MySQL
            // would store -1 instead of 0. The model's initializers already make new objects -1.
            builder.Property(mapObject => mapObject.Description).HasMaxLength(MapLimits.DescriptionMaxLength);

            builder.HasIndex(mapObject => mapObject.MapId);

            // An object belongs to its map: deleting the map deletes its objects.
            builder.HasOne(mapObject => mapObject.Map)
                   .WithMany(map => map.MapObjects)
                   .HasForeignKey(mapObject => mapObject.MapId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
