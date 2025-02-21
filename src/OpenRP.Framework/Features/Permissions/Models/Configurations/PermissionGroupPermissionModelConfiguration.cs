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
    public class PermissionGroupPermissionModelConfiguration : IEntityTypeConfiguration<PermissionGroupPermissionModel>
    {
        public void Configure(EntityTypeBuilder<PermissionGroupPermissionModel> builder)
        {
            // Set primary key
            builder.HasKey(pgpm => pgpm.Id);

            // Configure relationship to PermissionGroupModel
            builder.HasOne(pgpm => pgpm.PermissionGroup)
                   .WithMany(pgpm => pgpm.Permissions) 
                   .HasForeignKey(pgpm => pgpm.PermissionGroupId)
                   .OnDelete(DeleteBehavior.Cascade); // Adjust as needed

            // Configure relationship to PermissionModel
            builder.HasOne(pgpm => pgpm.Permission)
                   .WithMany() // Replace with a collection property if PermissionModel has one, e.g., .WithMany(p => p.PermissionGroupPermissions)
                   .HasForeignKey(pgpm => pgpm.PermissionId)
                   .OnDelete(DeleteBehavior.Cascade); // Adjust as needed
        }
    }
}
