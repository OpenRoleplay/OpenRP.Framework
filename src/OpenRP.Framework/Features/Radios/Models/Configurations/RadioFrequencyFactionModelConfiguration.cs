using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;

namespace OpenRP.Framework.Features.Radios.Models.Configurations
{
    public class RadioFrequencyFactionModelConfiguration : IEntityTypeConfiguration<RadioFrequencyFactionModel>
    {
        public void Configure(EntityTypeBuilder<RadioFrequencyFactionModel> builder)
        {
            // A faction is on a frequency's list once.
            builder.HasIndex(frequencyFaction => new { frequencyFaction.RadioFrequencyId, frequencyFaction.FactionId }).IsUnique();

            builder.HasOne(frequencyFaction => frequencyFaction.RadioFrequency)
                   .WithMany()
                   .HasForeignKey(frequencyFaction => frequencyFaction.RadioFrequencyId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(frequencyFaction => frequencyFaction.Faction)
                   .WithMany()
                   .HasForeignKey(frequencyFaction => frequencyFaction.FactionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
