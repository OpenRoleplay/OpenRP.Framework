using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Database.Services;
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

        public string GetName()
        {
            return _inventoryModel.Name;
        }

        public uint GetMaxWeight()
        {
            if (_inventoryModel.MaxWeight != null && _inventoryModel.MaxWeight.HasValue)
            {
                return _inventoryModel.MaxWeight.Value;
            }
            return uint.MaxValue;
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

        public uint GetAvailableWeight()
        {
            uint maxWeight = GetMaxWeight();

            if(GetMaxWeight() == 0)
            {
                return 0;
            }

            List<InventoryItem> inventoryItems = GetInventoryItems();
            if (inventoryItems == null || inventoryItems.Count == 0)
            {
                return maxWeight;
            }

            uint totalWeight = inventoryItems.Aggregate(0u, (sum, item) => sum + item.GetTotalWeight());

            uint availableWeight = maxWeight - totalWeight;

            return availableWeight > 0 ? availableWeight : 0;
        }

        public string GetInventoryDialogName(bool showWeight = true)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("{0}", GetName());

            if (showWeight)
            {
                sb.AppendFormat(" ({0}g / {1}g)", GetInventoryItems().Sum(i => i.GetTotalWeight()), GetMaxWeight());
            }

            return sb.ToString();
        }

        public bool DoesItemFit(Item item, uint amount)
        {

        }
    }
}
