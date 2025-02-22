using OpenRP.Framework.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Inventories.Services
{
    public interface IInventoryService
    {
        InventoryModel CreateInventory(string inventoryName, uint? maxWeightInGrams);
    }
}
