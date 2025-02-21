using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Permissions.Models.Configurations
{
    public class PermissionGroupModelConfiguration : IEntityTypeConfiguration<PermissionGroupModel>
    {
        public void Configure(EntityTypeBuilder<PermissionGroupModel> builder)
        {
            // Set primary key
            builder.HasKey(pg => pg.Id);

            // Configure required properties
            builder.Property(pg => pg.Name)
                   .IsRequired()
                   .HasMaxLength(100); // Adjust length as needed

            // Configure one-to-many relationship: one PermissionGroup has many Permissions
            builder.HasMany(pg => pg.Permissions)
                   .WithOne(pgpm => pgpm.PermissionGroup)
                   .HasForeignKey(pgpm => pgpm.PermissionGroupId)
                   .OnDelete(DeleteBehavior.Cascade); // Adjust delete behavior as needed
        }
    }
}
