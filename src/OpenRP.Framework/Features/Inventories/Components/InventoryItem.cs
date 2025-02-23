using OpenRP.Framework.Database.Models;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Inventories.Components
{
    public class InventoryItem : Component
    {
        private readonly InventoryItemModel _inventoryItemModel;
        public InventoryItem(InventoryItemModel inventoryItemModel)
        {
            _inventoryItemModel = inventoryItemModel;
        }
    }
}
