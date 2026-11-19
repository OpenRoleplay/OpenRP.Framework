using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;

namespace OpenRP.Framework.Features.Fishing.Models
{
    public class VehicleFishingNetModelConfiguration : IEntityTypeConfiguration<VehicleFishingNetModel>
    {
        public void Configure(EntityTypeBuilder<VehicleFishingNetModel> builder)
        {
            builder.ToTable("VehicleFishingNets");

            builder.HasKey(net => net.Id);
            builder.Property(net => net.Id)
                   .ValueGeneratedOnAdd();

            // A vehicle has one net at most.
            builder.HasIndex(net => net.VehicleId)
                   .IsUnique();

            builder.HasOne(net => net.Vehicle)
                   .WithMany()
                   .HasForeignKey(net => net.VehicleId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(net => net.StorageInventory)
                   .WithMany()
                   .HasForeignKey(net => net.StorageInventoryId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
