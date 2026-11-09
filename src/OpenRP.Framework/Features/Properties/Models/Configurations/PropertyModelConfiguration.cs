using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;

namespace OpenRP.Framework.Features.Properties.Models.Configurations
{
    public class PropertyModelConfiguration : IEntityTypeConfiguration<PropertyModel>
    {
        public void Configure(EntityTypeBuilder<PropertyModel> builder)
        {
            // Deleting a jurisdiction leaves its properties without one, instead of deleting them.
            builder.HasOne(property => property.Jurisdiction)
                   .WithMany()
                   .HasForeignKey(property => property.JurisdictionId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
