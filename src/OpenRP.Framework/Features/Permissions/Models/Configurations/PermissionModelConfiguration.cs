using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;

namespace OpenRP.Framework.Features.Permissions.Models.Configurations
{
    public class PermissionModelConfiguration : IEntityTypeConfiguration<PermissionModel>
    {
        private const int NameMaxLength = 128;

        public void Configure(EntityTypeBuilder<PermissionModel> builder)
        {
            builder.Property(permission => permission.Name)
                   .IsRequired()
                   .HasMaxLength(NameMaxLength);

            // Permissions are created at startup by name; this keeps two servers starting together from creating one twice.
            builder.HasIndex(permission => permission.Name)
                   .IsUnique();
        }
    }
}
