using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;

namespace OpenRP.Framework.Features.Factions.Models.Configurations
{
    public class FactionPermissionModelConfiguration : IEntityTypeConfiguration<FactionPermissionModel>
    {
        public void Configure(EntityTypeBuilder<FactionPermissionModel> builder)
        {
            builder.HasOne(factionPermission => factionPermission.Faction)
                   .WithMany()
                   .HasForeignKey(factionPermission => factionPermission.FactionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(factionPermission => factionPermission.Permission)
                   .WithMany()
                   .HasForeignKey(factionPermission => factionPermission.PermissionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(factionPermission => new { factionPermission.FactionId, factionPermission.PermissionId })
                   .IsUnique();
        }
    }
}
