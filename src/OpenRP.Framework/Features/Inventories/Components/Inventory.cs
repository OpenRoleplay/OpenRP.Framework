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
        private const string DefaultWalletKey = "DEFAULT_WALLET";
        private const string WearingKey = "WEARING";

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
            return GetComponentsInChildren<InventoryItem>().Where(i => !i.IsDeleted()).ToList();
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

                    if(amount != 0 && !inventoryItemToCompareWith.HasAmount(amount))
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

                if (!IsUnchangedCopyOf(inventoryItemToCompareWith, item))
                {
                    continue;
                }

                if (amount != 0 && !inventoryItemToCompareWith.HasAmount(amount))
                {
                    continue;
                }

                return inventoryItemToCompareWith;
            }
            return null;
        }

        /// <summary>
        /// True when the inventory item is this item with its default properties (not personalised, no custom weight).
        /// </summary>
        private static bool IsUnchangedCopyOf(InventoryItem inventoryItem, Item item)
        {
            ItemModel itemModel = item.GetRawItemModel();
            InventoryItemModel inventoryItemModel = inventoryItem.GetRawInventoryItemModel();

            return inventoryItem.GetItem().GetId() == item.GetId()
                && itemModel.CanDestroy == inventoryItemModel.CanDestroy
                && itemModel.CanDrop == inventoryItemModel.CanDrop
                && itemModel.KeepOnDeath == inventoryItemModel.KeepOnDeath
                && inventoryItemModel.Weight == null;
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

        /// <summary>
        /// Adds the item, on top of a matching stack when the item is stackable. False when it doesn't fit.
        /// </summary>
        public bool AddItem(Item item, uint amount, ItemAdditionalData? customAdditionalData = null)
        {
            if (!DoesItemFit(item, amount))
            {
                return false;
            }

            string? additionalData = GetAdditionalDataForNewItem(item, customAdditionalData);
            InventoryItem? existingStack = item.IsStackable() ? FindStack(item, additionalData) : null;
            if (existingStack != null)
            {
                existingStack.Add(amount);
                return true;
            }

            CreateInventoryItem(item, amount, additionalData);
            return true;
        }

        private static string? GetAdditionalDataForNewItem(Item item, ItemAdditionalData? customAdditionalData)
        {
            string requestedAdditionalData = customAdditionalData?.ToString().Trim() ?? string.Empty;
            if (requestedAdditionalData.Length == 0)
            {
                return null;
            }

            string itemAdditionalData = item.GetRawItemModel().UseValue?.Trim() ?? string.Empty;
            return ItemAdditionalData.Combine(requestedAdditionalData, itemAdditionalData).ToString();
        }

        private InventoryItem? FindStack(Item item, string? additionalData)
        {
            return GetInventoryItems().FirstOrDefault(inventoryItem =>
                IsUnchangedCopyOf(inventoryItem, item)
                && ItemAdditionalData.Equals(inventoryItem.GetRawInventoryItemModel().AdditionalData, additionalData));
        }

        private void CreateInventoryItem(Item item, uint amount, string? additionalData)
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
                AdditionalData = additionalData,
            };

            EntityId newInventoryItemEntityId = UnsavedEntities.GenerateUnsavedId();

            Manager.Create(newInventoryItemEntityId, Entity);
            Manager.AddComponent<InventoryItem>(newInventoryItemEntityId, inventoryItemModel);
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
            InventoryItem? inventoryItemToModify = FindItem(inventoryItem, 1);
            if (inventoryItemToModify != null)
            {
                inventoryItemToModify.Use();
                return true;
            }
            return false;
        }

        /// <summary>
        /// How many of this item are in this inventory, counted over all of its stacks.
        /// </summary>
        public uint GetItemAmount(Item item)
        {
            return (uint)GetStacksOf(item).Sum(inventoryItem => inventoryItem.GetAmount());
        }

        /// <summary>
        /// Removes this amount of the item, spread over its stacks. Removes nothing and returns false when there isn't enough.
        /// </summary>
        public bool TakeItem(Item item, uint amount)
        {
            if (GetItemAmount(item) < amount)
            {
                return false;
            }

            // InventoryItem.Subtract(0) means "everything", so taking nothing must not reach it.
            if (amount == 0)
            {
                return true;
            }

            uint amountLeft = amount;
            foreach (InventoryItem stack in GetStacksOf(item))
            {
                amountLeft -= TakeFromStack(stack, amountLeft);
                if (amountLeft == 0)
                {
                    break;
                }
            }
            return true;
        }

        private List<InventoryItem> GetStacksOf(Item item)
        {
            return GetInventoryItems().Where(inventoryItem => inventoryItem.GetItem().GetId() == item.GetId()).ToList();
        }

        private static uint TakeFromStack(InventoryItem stack, uint amountWanted)
        {
            uint amountTaken = Math.Min(stack.GetAmount(), amountWanted);
            stack.Subtract(amountTaken);
            if (stack.GetAmount() == 0)
            {
                stack.ProcessDeletion();
            }
            return amountTaken;
        }

        /// <summary>
        /// The wallet items directly in this inventory.
        /// </summary>
        public List<InventoryItem> GetWallets()
        {
            return GetInventoryItems().Where(inventoryItem => inventoryItem.GetItem().IsItemWallet()).ToList();
        }

        /// <summary>
        /// The contents of the wallet marked as default, or null when no wallet is marked.
        /// </summary>
        public Inventory? GetDefaultWallet()
        {
            return GetWallets().FirstOrDefault(wallet => wallet.HasAdditionalDataFlag(DefaultWalletKey))?.GetItemInventory();
        }

        /// <summary>
        /// Marks this wallet as the default one and unmarks every other wallet in this inventory.
        /// </summary>
        public void SetDefaultWallet(InventoryItem wallet)
        {
            foreach (InventoryItem otherWallet in GetWallets())
            {
                otherWallet.SetAdditionalDataFlag(DefaultWalletKey, false);
            }
            wallet.SetAdditionalDataFlag(DefaultWalletKey, true);
        }

        /// <summary>
        /// The skin items directly in this inventory.
        /// </summary>
        public List<InventoryItem> GetSkins()
        {
            return GetInventoryItems().Where(inventoryItem => inventoryItem.GetItem().IsItemSkin()).ToList();
        }

        /// <summary>
        /// Marks this skin as worn and every other skin in this inventory as not worn.
        /// </summary>
        public void SetWornSkin(InventoryItem skin)
        {
            foreach (InventoryItem otherSkin in GetSkins())
            {
                otherSkin.SetAdditionalDataFlag(WearingKey, false);
            }
            skin.SetAdditionalDataFlag(WearingKey, true);
        }
    }
}
