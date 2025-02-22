using OpenRP.Framework.Database.Models;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Inventories.Components
{
    public class Inventory : Component
    {
        private InventoryModel _inventoryModel;
        public Inventory(InventoryModel inventoryModel)
        {
            _inventoryModel = inventoryModel;
        }
    }
}
