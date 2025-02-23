using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Items.Components;
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
        private bool _scheduledForSaving;
        public Inventory(InventoryModel inventoryModel)
        {
            _inventoryModel = inventoryModel;
        }

        public bool IsScheduledForSaving()
        {
            return _scheduledForSaving;
        }

        public void ScheduleForSaving(bool save = true)
        {
            _scheduledForSaving = save;
        }

        public ulong GetId()
        {
            return _inventoryModel.Id;
        }

        public uint? GetMaxWeight()
        {
            return _inventoryModel.MaxWeight;
        }

        public List<InventoryItem> GetInventoryItems()
        {
            return GetComponentsInChildren<InventoryItem>().ToList();
        }

        public Inventory GetParentInventory()
        {
            List<Inventory> allInventories = Manager.GetComponents<Inventory>().ToList();

            return allInventories.SingleOrDefault(inv =>
                inv.GetInventoryItems().Any(item =>
                {
                    Item itemInstance = item.GetItem();
                    // Check that this item is an inventory item and that its AdditionalData contains a reference to the given inventory's ID.
                    return itemInstance.IsItemInventory() &&
                        item.GetAdditionalData().GetString("INVENTORY") == inv.GetId().ToString();
                }));
        }

        public bool DoesItemFit(Item item, uint amount)
        {

        }
    }
}
