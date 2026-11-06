using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;

namespace OpenRP.Framework.Features.Factions.Models.Configurations
{
    public class FactionMemberModelConfiguration : IEntityTypeConfiguration<FactionMemberModel>
    {
        public void Configure(EntityTypeBuilder<FactionMemberModel> builder)
        {
            builder.HasOne(member => member.Character)
                   .WithMany(character => character.Factions)
                   .HasForeignKey(member => member.CharacterId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(member => member.Faction)
                   .WithMany()
                   .HasForeignKey(member => member.FactionId)
                   .OnDelete(DeleteBehavior.Cascade);

            // In-game, only ranks without members can be deleted; this only matters when a faction is deleted in the database.
            builder.HasOne(member => member.FactionRank)
                   .WithMany()
                   .HasForeignKey(member => member.FactionRankId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(member => new { member.CharacterId, member.FactionId })
                   .IsUnique();
        }
    }
}
