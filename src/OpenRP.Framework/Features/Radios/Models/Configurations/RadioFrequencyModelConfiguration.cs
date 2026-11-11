using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;

namespace OpenRP.Framework.Features.Radios.Models.Configurations
{
    public class RadioFrequencyModelConfiguration : IEntityTypeConfiguration<RadioFrequencyModel>
    {
        public void Configure(EntityTypeBuilder<RadioFrequencyModel> builder)
        {
            builder.Property(frequency => frequency.Name).HasMaxLength(32).IsRequired();

            // A frequency has one set of rules.
            builder.HasIndex(frequency => frequency.Number).IsUnique();
        }
    }
}
