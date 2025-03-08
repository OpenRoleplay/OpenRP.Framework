using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Inventories.Models.DefaultData
{
    public class InventoryModelDefaultData : IEntityTypeConfiguration<InventoryModel>
    {
        public void Configure(EntityTypeBuilder<InventoryModel> builder)
        {
            builder.HasData(
                new InventoryModel
                {
                    Id = 1
                    , Name = "World Inventory"
                    , MaxWeight = null,
                }
            );
        }
    }
}
