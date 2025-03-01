using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Database.Services;
using OpenRP.Framework.Features.Inventories.Entities;
using OpenRP.Framework.Features.Inventories.Helpers;
using OpenRP.Framework.Features.Items.Components;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Inventories.Components
{
    public class Inventory : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private InventoryModel _inventoryModel;
        private bool _hasChanges;
        private bool _isDeleted;
        private EntityId? _parentInventoryEntityId;
        public Inventory(InventoryModel inventoryModel)
        {
            _inventoryModel = inventoryModel;
            _parentInventoryEntityId = null;
        }

        public bool HasChanges()
        {
            return _hasChanges;
        }

        public void ProcessChanges(bool processChanges = true)
        {
            _hasChanges = processChanges;
        }

        public bool IsDeleted()
        {
            return _isDeleted;
        }

        public void ProcessDeletion(bool processDeletion = true)
        {
            _isDeleted = processDeletion;
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

        public Inventory? GetParentInventory()
        {
            if(_parentInventoryEntityId.HasValue)
            {
                return Manager.GetComponent<Inventory>(_parentInventoryEntityId.Value);
            }

            // Retrieve all Inventory components.
            List<Inventory> allInventories = Manager.GetComponents<Inventory>().ToList();

            // Loop through each inventory.
            foreach (Inventory inventory in allInventories)
            {
                // Loop through each inventory item in the current inventory.
                foreach (InventoryItem item in inventory.GetInventoryItems())
                {
                    Item itemInstance = item.GetItem();

                    // Only consider inventory items.
                    if (!itemInstance.IsItemInventory())
                    {
                        continue;
                    }

                    // Get the reference stored in the item's additional data.
                    string referencedInventoryId = item.GetAdditionalData().GetString("INVENTORY");
                    string currentInventoryId = inventory.GetId().ToString();

                    if (referencedInventoryId == currentInventoryId)
                    {
                        _parentInventoryEntityId = inventory.Entity;

                        return inventory;
                    }
                }
            }

            // Return null if no matching parent inventory is found.
            return null;
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

        public bool HasItem(Item? item, uint amount = 0)
        {
            if (item != null)
            {
                List<InventoryItem> inventoryItems = GetInventoryItems();

                if (inventoryItems.Any(i => i.GetItem().GetId() == item.GetId()
                    && (amount == 0 || i.GetAmount() >= amount)
                ))
                {
                    return true;
                }
            }
            return false;
        }

        public InventoryItem? FindItem(InventoryItem inventoryItem, uint amount = 0)
        {
            List<InventoryItem> inventoryItems = GetInventoryItems();

            foreach (InventoryItem inventoryItemToCompareWith in inventoryItems)
            {
                if (inventoryItemToCompareWith.IsDeleted())
                {
                    continue;
                }

                if (inventoryItem.GetItem().GetId() == inventoryItemToCompareWith.GetItem().GetId())
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

        public InventoryItem? FindItem(Item item, uint amount = 0)
        {
            List<InventoryItem> inventoryItems = GetInventoryItems();

            foreach (InventoryItem inventoryItemToCompareWith in inventoryItems)
            {
                if(inventoryItemToCompareWith.IsDeleted())
                {
                    continue;
                }

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

        public bool DoesInventoryItemFit(InventoryItem inventoryItemToCheck, uint amountToCheck)
        {
            // Check if amount of inventoryitem fits in the inventory 
            if (inventoryItemToCheck.GetTotalWeight(amountToCheck) < GetAvailableWeight())
            {
                return true;
            }
            return false;
        }

        public bool DoesItemFit(Item itemToCheck, uint amountToCheck)
        {
            // Check if amount of inventoryitem fits in the inventory 
            if ((itemToCheck.GetWeight() * amountToCheck) < GetAvailableWeight())
            {
                return true;
            }
            return false;
        }

        public bool AddItem(Item item, uint amount, ItemAdditionalData? customAdditionalData = null)
        {
            if (DoesItemFit(item, amount))
            {
                bool hasItem = HasItem(item, amount);

                if (hasItem && !item.IsStackable())
                {
                    InventoryItem? existingItem = FindItem(item, amount);

                    if (existingItem != null)
                    {
                        existingItem.Add(amount);
                    }
                } 
                else
                {
                    ItemModel itemModel = item.GetRawItemModel();

                    InventoryItemModel inventoryItemModel = new InventoryItemModel()
                    {
                        ItemId = item.GetId(),
                        Amount = amount,
                        CanDestroy = itemModel.CanDestroy,
                        CanDrop = itemModel.CanDrop,
                        KeepOnDeath = itemModel.KeepOnDeath,
                        InventoryId = GetId(),
                        UsesRemaining = itemModel.MaxUses,
                    };

                    if (customAdditionalData != null)
                    {
                        string requestedAdditionalData = customAdditionalData.ToString().Trim();
                        string itemAdditionalData = itemModel.UseValue.Trim();

                        string combined = ItemAdditionalData.Combine(requestedAdditionalData, itemAdditionalData).ToString();

                        inventoryItemModel.AdditionalData = combined;
                    }

                    int newInventoryItemId = InventoryNewEntities.GenerateNewInventoryItemId();
                    EntityId newInventoryItemEntityId = InventoryEntities.GetInventoryItemId(newInventoryItemId);

                    Manager.Create(newInventoryItemEntityId, Entity);
                    Manager.AddComponent<InventoryItem>(newInventoryItemEntityId, inventoryItemModel);
                }
            }
            return false;
        }

        public bool RemoveItem(Item item, uint amount = 0)
        {
            // Attempt to find the matching InventoryItem in the inventory.
            InventoryItem? inventoryItem = FindItem(item, amount);
            if (inventoryItem == null)
            {
                // Item not found, nothing to remove.
                return false;
            }

            uint currentAmount = inventoryItem.GetAmount();

            // If no specific amount is provided or the removal amount is greater than or equal to what's available,
            // remove the item entirely.
            if (amount == 0 || currentAmount <= amount)
            {
                inventoryItem.ProcessDeletion();
            }
            else
            {
                // Otherwise, remove only the specified amount.
                // (Assumes InventoryItem has a Remove method; if not, adjust the amount accordingly.)
                inventoryItem.Subtract(amount);
            }

            return true;
        }

        public bool UseItem(Player player, InventoryItem inventoryItem)
        {
            // Retrieve the underlying model from the component.
            InventoryItemModel inventoryItemModel = inventoryItem.GetRawInventoryItemModel();

            // If the item tracks uses, decrement the count.
            if (inventoryItemModel.UsesRemaining != null)
            {
                inventoryItemModel.UsesRemaining--;

                // If no uses remain, remove the item from the system.
                if (inventoryItemModel.UsesRemaining == 0)
                {
                    inventoryItem.ProcessDeletion();
                }
            }
            return true;
        }

    }
}
