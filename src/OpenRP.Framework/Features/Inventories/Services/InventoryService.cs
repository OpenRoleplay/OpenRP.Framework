using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Inventories.Services
{
    public class InventoryService : IInventoryService
    {
        private BaseDataContext _dataContext;
        public InventoryService(BaseDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public InventoryModel CreateInventory(string inventoryName, uint? maxWeightInGrams)
        {
            try
            {
                InventoryModel newInventory = new InventoryModel() { Name = inventoryName, MaxWeight = maxWeightInGrams };
                _dataContext.Inventories.Add(newInventory);
                _dataContext.SaveChanges();

                return newInventory;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }
    }
}
