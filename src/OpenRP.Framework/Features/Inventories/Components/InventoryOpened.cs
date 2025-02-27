using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Inventories.Components
{
    public class InventoryOpened : Component
    {
        public Inventory OpenedInventory;
        public Dictionary<uint, InventoryItem> OpenedInventoryItems;

        public InventoryOpened(Inventory inventory, Dictionary<uint, InventoryItem> inventoryItems) 
        { 
            this.OpenedInventory = inventory;
            this.OpenedInventoryItems = inventoryItems;
        }
    }
}
