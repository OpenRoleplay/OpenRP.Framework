using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;

namespace OpenRP.Framework.Features.Jurisdictions.Models.Configurations
{
    public class JurisdictionModelConfiguration : IEntityTypeConfiguration<JurisdictionModel>
    {
        public void Configure(EntityTypeBuilder<JurisdictionModel> builder)
        {
            builder.Property(jurisdiction => jurisdiction.Name).HasMaxLength(64).IsRequired();
            builder.Property(jurisdiction => jurisdiction.TwoLetterTag).HasMaxLength(2).IsRequired();
            builder.Property(jurisdiction => jurisdiction.ThreeLetterTag).HasMaxLength(3).IsRequired();
            builder.Property(jurisdiction => jurisdiction.FourLetterTag).HasMaxLength(4).IsRequired();

            // Tags identify a jurisdiction (plates, and later bank account numbers), so no two may share one.
            builder.HasIndex(jurisdiction => jurisdiction.TwoLetterTag).IsUnique();
            builder.HasIndex(jurisdiction => jurisdiction.ThreeLetterTag).IsUnique();
            builder.HasIndex(jurisdiction => jurisdiction.FourLetterTag).IsUnique();

            // A jurisdiction with children can't be deleted; the editor refuses it too.
            builder.HasOne(jurisdiction => jurisdiction.ParentJurisdiction)
                   .WithMany()
                   .HasForeignKey(jurisdiction => jurisdiction.ParentJurisdictionId)
                   .OnDelete(DeleteBehavior.Restrict);

            // A deleted currency or governing faction leaves the jurisdiction without one; it then uses its parent's.
            builder.HasOne(jurisdiction => jurisdiction.Currency)
                   .WithMany()
                   .HasForeignKey(jurisdiction => jurisdiction.CurrencyId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(jurisdiction => jurisdiction.GovernmentFaction)
                   .WithMany()
                   .HasForeignKey(jurisdiction => jurisdiction.GovernmentFactionId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
