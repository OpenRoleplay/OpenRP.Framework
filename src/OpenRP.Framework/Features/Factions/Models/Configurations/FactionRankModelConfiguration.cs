using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;

namespace OpenRP.Framework.Features.Factions.Models.Configurations
{
    public class FactionRankModelConfiguration : IEntityTypeConfiguration<FactionRankModel>
    {
        private const int NameMaxLength = 64;

        public void Configure(EntityTypeBuilder<FactionRankModel> builder)
        {
            builder.Property(rank => rank.Name)
                   .IsRequired()
                   .HasMaxLength(NameMaxLength);

            builder.HasOne(rank => rank.Faction)
                   .WithMany()
                   .HasForeignKey(rank => rank.FactionId)
                   .OnDelete(DeleteBehavior.Cascade);

            // In-game, only ranks without sub-ranks can be deleted; this only matters when a faction is deleted in the database.
            builder.HasOne(rank => rank.ParentRank)
                   .WithMany()
                   .HasForeignKey(rank => rank.ParentRankId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
