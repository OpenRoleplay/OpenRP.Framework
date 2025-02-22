using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Inventories.Entities
{
    public static class InventoryEntities
    {
        [EntityType]
        public static readonly Guid InventoryType = new Guid("AF4DA929-422B-4496-8583-12F1C53B0F1C");

        [EntityType]
        public static readonly Guid InventoryItemType = new Guid("08DBF255-2A7C-48C5-9FBD-49CF7BF20092");

        public static EntityId GetInventoryId(int inventoryId)
        {
            return new EntityId(InventoryType, inventoryId);
        }

        public static EntityId GetInventoryItemId(int inventoryItemId)
        {
            return new EntityId(InventoryItemType, inventoryItemId);
        }
    }
}
