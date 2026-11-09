using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;

namespace OpenRP.Framework.Features.Jurisdictions.Models.Configurations
{
    public class FactionTaxRegistrationModelConfiguration : IEntityTypeConfiguration<FactionTaxRegistrationModel>
    {
        public void Configure(EntityTypeBuilder<FactionTaxRegistrationModel> builder)
        {
            // A faction is registered in a jurisdiction at most once.
            builder.HasIndex(registration => new { registration.FactionId, registration.JurisdictionId }).IsUnique();

            // A registration means nothing without its faction or jurisdiction.
            builder.HasOne(registration => registration.Faction)
                   .WithMany()
                   .HasForeignKey(registration => registration.FactionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(registration => registration.Jurisdiction)
                   .WithMany()
                   .HasForeignKey(registration => registration.JurisdictionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
