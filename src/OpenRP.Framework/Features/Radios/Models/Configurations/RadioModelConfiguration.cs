using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;

namespace OpenRP.Framework.Features.Radios.Models.Configurations
{
    public class RadioModelConfiguration : IEntityTypeConfiguration<RadioModel>
    {
        public void Configure(EntityTypeBuilder<RadioModel> builder)
        {
            // A vehicle has at most one radio installed.
            builder.HasIndex(radio => radio.VehicleId).IsUnique();

            builder.HasOne(radio => radio.Vehicle)
                   .WithMany()
                   .HasForeignKey(radio => radio.VehicleId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
