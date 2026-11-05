using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Shared.BaseManager.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Inventories.Services
{
    public interface IInventoryManager : IBaseManager
    {
        /// <summary>
        /// Saves a new, empty inventory right away and loads it, so it can be used immediately.
        /// </summary>
        Inventory CreateInventory(string inventoryName, uint? maxWeightInGrams);

        /// <summary>
        /// The inventory with this id, loading it first when it isn't loaded yet. Null when it doesn't exist.
        /// </summary>
        Inventory? LoadInventory(ulong inventoryId);
    }
}
