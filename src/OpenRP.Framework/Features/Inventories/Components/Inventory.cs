using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Database.Services;
using OpenRP.Framework.Features.Inventories.Helpers;
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
        private bool _hasChanges;
        public Inventory(InventoryModel inventoryModel)
        {
            _inventoryModel = inventoryModel;
        }

        public bool HasChanges()
        {
            return _hasChanges;
        }

        public void ProcessChanges(bool processChanges = true)
        {
            _hasChanges = processChanges;
        }

        public InventoryModel GetRawInventoryModel()
        {
            return _inventoryModel;
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

        public bool HasItem(Item item, uint amount = 0)
        {
            List<InventoryItem> inventoryItems = GetInventoryItems();

            if(inventoryItems.Any(i => i.GetItem().GetId() == item.GetId()
                && (amount != 0 && i.GetAmount() >= amount) 
            ))
            {
                return true;
            }
            return false;
        }

        public InventoryItem FindItem(InventoryItem inventoryItem, uint amount = 0)
        {
            List<InventoryItem> inventoryItems = GetInventoryItems();

            foreach (InventoryItem inventoryItemToCompareWith in inventoryItems)
            {
                if(inventoryItem.GetItem().GetId() == inventoryItemToCompareWith.GetItem().GetId())
                {
                    InventoryItemModel inventoryItemModel = inventoryItem.GetRawInventoryItemModel();
                    InventoryItemModel inventoryItemModelToCompareWith = inventoryItemToCompareWith.GetRawInventoryItemModel();

                    if (inventoryItemModel.CanDestroy != inventoryItemModelToCompareWith.CanDestroy)
                    {
                        continue;
                    }

                    if (inventoryItemModel.CanDrop != inventoryItemModelToCompareWith.CanDrop)
                    {
                        continue;
                    }

                    if (inventoryItemModel.KeepOnDeath != inventoryItemModelToCompareWith.KeepOnDeath)
                    {
                        continue;
                    }

                    if (inventoryItemModel.UsesRemaining != inventoryItemModelToCompareWith.UsesRemaining)
                    {
                        continue;
                    }

                    if (inventoryItemModel.Weight != inventoryItemModelToCompareWith.Weight)
                    {
                        continue;
                    }

                    if(!ItemAdditionalData.Equals(inventoryItemModel.AdditionalData, inventoryItemModelToCompareWith.AdditionalData))
                    {
                        continue;
                    }

                    if(amount != 0 && inventoryItemModelToCompareWith.Amount >= amount)
                    {
                        continue;
                    }

                    return inventoryItemToCompareWith;
                }
            }
            return null;
        }

        public InventoryItem FindItem(Item item, uint amount = 0)
        {
            List<InventoryItem> inventoryItems = GetInventoryItems();

            foreach (InventoryItem inventoryItemToCompareWith in inventoryItems)
            {
                if (item.GetId() == inventoryItemToCompareWith.GetItem().GetId())
                {
                    ItemModel itemModel = item.GetRawItemModel();
                    ItemModel itemModelToCompareWith = inventoryItemToCompareWith.GetItem().GetRawItemModel();
                    InventoryItemModel inventoryItemModelToCompareWith = inventoryItemToCompareWith.GetRawInventoryItemModel();

                    if (itemModel.Id != itemModelToCompareWith.Id)
                    {
                        continue;
                    }

                    if (itemModel.CanDestroy != inventoryItemModelToCompareWith.CanDestroy)
                    {
                        continue;
                    }

                    if (itemModel.CanDrop != inventoryItemModelToCompareWith.CanDrop)
                    {
                        continue;
                    }

                    if (itemModel.KeepOnDeath != inventoryItemModelToCompareWith.KeepOnDeath)
                    {
                        continue;
                    }

                    if (itemModel.MaxUses != inventoryItemModelToCompareWith.UsesRemaining)
                    {
                        continue;
                    }

                    if (inventoryItemModelToCompareWith.Weight != null)
                    {
                        continue;
                    }

                    if (amount != 0 && inventoryItemToCompareWith.HasAmount(amount))
                    {
                        continue;
                    }

                    return inventoryItemToCompareWith;
                }
            }
            return null;
        }

        public bool HasItem(InventoryItem inventoryItem, uint amount = 0)
        {
            if (FindItem(inventoryItem, amount) != null)
            {
                return true;
            }
            return false;
        }
    }
}
