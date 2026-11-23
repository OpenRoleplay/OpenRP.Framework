using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;

namespace OpenRP.Framework.Features.Jurisdictions.Models.Configurations
{
    public class JurisdictionSettingModelConfiguration : IEntityTypeConfiguration<JurisdictionSettingModel>
    {
        public void Configure(EntityTypeBuilder<JurisdictionSettingModel> builder)
        {
            builder.Property(setting => setting.Key).HasMaxLength(64).IsRequired();
            builder.Property(setting => setting.Value).HasMaxLength(64);

            // A jurisdiction has at most one row per setting.
            builder.HasIndex(setting => new { setting.JurisdictionId, setting.Key }).IsUnique();

            // A setting means nothing without its jurisdiction.
            builder.HasOne(setting => setting.Jurisdiction)
                   .WithMany()
                   .HasForeignKey(setting => setting.JurisdictionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
