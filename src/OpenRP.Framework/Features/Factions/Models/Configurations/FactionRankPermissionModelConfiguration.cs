using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;

namespace OpenRP.Framework.Features.Factions.Models.Configurations
{
    public class FactionRankPermissionModelConfiguration : IEntityTypeConfiguration<FactionRankPermissionModel>
    {
        public void Configure(EntityTypeBuilder<FactionRankPermissionModel> builder)
        {
            builder.HasOne(rankPermission => rankPermission.FactionRank)
                   .WithMany()
                   .HasForeignKey(rankPermission => rankPermission.FactionRankId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rankPermission => rankPermission.Permission)
                   .WithMany()
                   .HasForeignKey(rankPermission => rankPermission.PermissionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(rankPermission => new { rankPermission.FactionRankId, rankPermission.PermissionId })
                   .IsUnique();
        }
    }
}
