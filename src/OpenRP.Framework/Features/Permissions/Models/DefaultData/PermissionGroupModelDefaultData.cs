using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Permissions.Models.DefaultData
{
    public class PermissionGroupModelDefaultData : IEntityTypeConfiguration<PermissionGroupModel>
    {
        public void Configure(EntityTypeBuilder<PermissionGroupModel> builder)
        {
            builder.HasData(
                new PermissionGroupModel
                {
                    Id = 1
                    , Name = "Default"
                },
                new PermissionGroupModel
                {
                    Id = 2
                    , Name = "Admin"
                },
                new PermissionGroupModel
                {
                    Id = 3
                    , Name = "Tester"
                }
            );
        }
    }
}
