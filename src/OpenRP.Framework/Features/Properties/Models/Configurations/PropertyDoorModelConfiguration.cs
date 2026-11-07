using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;

namespace OpenRP.Framework.Features.Properties.Models.Configurations
{
    public class PropertyDoorModelConfiguration : IEntityTypeConfiguration<PropertyDoorModel>
    {
        public void Configure(EntityTypeBuilder<PropertyDoorModel> builder)
        {
            builder.HasOne(door => door.Property)
                   .WithMany(property => property.PropertyDoors)
                   .HasForeignKey(door => door.PropertyId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Several doors may lead to the same door, for example a front and a back entrance into one interior.
            // Deleting a door makes the doors that led to it lead nowhere.
            builder.HasOne(door => door.LinkedToPropertyDoor)
                   .WithMany()
                   .HasForeignKey(door => door.LinkedToPropertyDoorId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
