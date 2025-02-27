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
        public Dictionary<int, InventoryItem> OpenedInventoryItems;
        public InventoryItem? OpenedInventoryItem;

        public InventoryOpened(Inventory inventory) 
        { 
            this.OpenedInventory = inventory;
            this.OpenedInventoryItems = new Dictionary<int, InventoryItem>();
        }
    }
}
