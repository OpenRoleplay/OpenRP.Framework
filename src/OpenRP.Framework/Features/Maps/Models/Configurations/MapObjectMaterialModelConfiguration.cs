using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;

namespace OpenRP.Framework.Features.Maps.Models.Configurations
{
    public class MapObjectMaterialModelConfiguration : IEntityTypeConfiguration<MapObjectMaterialModel>
    {
        public void Configure(EntityTypeBuilder<MapObjectMaterialModel> builder)
        {
            builder.Property(material => material.TxdName).HasMaxLength(64);
            builder.Property(material => material.TextureName).HasMaxLength(64);
            builder.Property(material => material.Text).HasMaxLength(256);
            builder.Property(material => material.FontFace).HasMaxLength(64);

            // An object has at most one material per slot.
            builder.HasIndex(material => new { material.MapObjectId, material.MaterialIndex }).IsUnique();

            // A material means nothing without its object.
            builder.HasOne(material => material.MapObject)
                   .WithMany(mapObject => mapObject.Materials)
                   .HasForeignKey(material => material.MapObjectId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
