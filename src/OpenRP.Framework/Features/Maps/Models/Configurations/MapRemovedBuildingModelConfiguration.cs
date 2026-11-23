using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;

namespace OpenRP.Framework.Features.Maps.Models.Configurations
{
    public class MapRemovedBuildingModelConfiguration : IEntityTypeConfiguration<MapRemovedBuildingModel>
    {
        public void Configure(EntityTypeBuilder<MapRemovedBuildingModel> builder)
        {
            builder.HasIndex(removal => removal.MapId);

            // A removed building belongs to its map: deleting the map deletes its removals.
            builder.HasOne(removal => removal.Map)
                   .WithMany(map => map.MapRemovedBuildings)
                   .HasForeignKey(removal => removal.MapId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
