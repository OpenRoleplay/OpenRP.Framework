using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Database.Services;
using OpenRP.Framework.Features.Inventories.Entities;
using OpenRP.Framework.Features.Inventories.Helpers;
using OpenRP.Framework.Features.Items.Components;
using OpenRP.Framework.Features.Items.Entities;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Inventories.Components
{
    public class InventoryItem : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private const string InventoryKey = "INVENTORY";
        private const string SkinKey = "SKIN";

        private readonly InventoryItemModel _inventoryItemModel;
        private bool _hasChanges;
        private bool _isDeleted;
        public InventoryItem(InventoryItemModel inventoryItemModel)
        {
            _inventoryItemModel = inventoryItemModel;
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
            if(_inventoryItemModel.Id == 0)
            {
                DestroyEntity();
                return;
            }
            _isDeleted = processDeletion;
        }

        public InventoryItemModel GetRawInventoryItemModel()
        {
            return _inventoryItemModel;
        }

        public ulong GetId()
        {
            return _inventoryItemModel.Id;
        }

        public Item GetItem()
        {
            EntityId entityId = ItemEntities.GetItemId((int)_inventoryItemModel.ItemId);

            return Manager.GetComponent<Item>(entityId);
        }

        public string GetName()
        {
            return GetItem().GetName();
        }

        public Inventory GetInventoryIn()
        {
            return GetComponentInParent<Inventory>();
        }

        public uint GetWeight()
        {
            if(_inventoryItemModel.Weight != null)
            {
                return _inventoryItemModel.Weight.Value;
            }

            return GetItem().GetWeight();
        }

        public uint GetAmount()
        {
            return _inventoryItemModel.Amount;
        }

        public bool HasMaxUses()
        {
            return _inventoryItemModel.UsesRemaining != null;
        }

        public bool HasRemainingUses()
        {
            return _inventoryItemModel.UsesRemaining.HasValue && _inventoryItemModel.UsesRemaining.Value > 0;
        }

        public void Use()
        {
            _inventoryItemModel.UsesRemaining--;

            if( _inventoryItemModel.UsesRemaining > 0)
            {
                ProcessChanges();
            } else
            {
                ProcessChanges(false);
                ProcessDeletion();
            }
        }

        public uint? GetRemainingUses()
        {
            return _inventoryItemModel.UsesRemaining;
        }

        public bool HasAmount(uint amount)
        {
            return GetAmount() >= amount;
        }

        public bool Subtract(uint amount)
        {
            if(HasAmount(amount))
            {
                if (amount == 0)
                {
                    amount = _inventoryItemModel.Amount;
                }
                _inventoryItemModel.Amount -= amount; 
                ProcessChanges();
                return true;
            }
            return false;
        }

        public bool Add(uint amount)
        {
            _inventoryItemModel.Amount += amount;
            ProcessChanges();
            return true;
        }

        public ItemAdditionalData GetAdditionalData()
        {
            return ItemAdditionalData.Parse(_inventoryItemModel.AdditionalData);
        }

        /// <summary>
        /// True when the boolean flag (for example DEFAULT_WALLET or WEARING) is set in this item's additional data.
        /// </summary>
        public bool HasAdditionalDataFlag(string key)
        {
            return GetAdditionalData().GetBoolean(key) == true;
        }

        /// <summary>
        /// Sets a boolean flag in this item's additional data and marks the item as changed.
        /// </summary>
        public void SetAdditionalDataFlag(string key, bool value)
        {
            if (HasAdditionalDataFlag(key) == value)
            {
                return;
            }

            ItemAdditionalData additionalData = GetAdditionalData();
            additionalData.SetBoolean(key, value);
            _inventoryItemModel.AdditionalData = additionalData.ToString();
            ProcessChanges();
        }

        /// <summary>
        /// The skin model this clothing item puts on the character, or null when it isn't a skin item.
        /// </summary>
        public int? GetSkinModelId()
        {
            if (!GetItem().IsItemSkin())
            {
                return null;
            }

            return GetAdditionalData().GetInt(SkinKey)
                ?? ItemAdditionalData.Parse(GetItem().GetRawItemModel().UseValue).GetInt(SkinKey);
        }

        /// <summary>
        /// The inventory inside this item, such as the contents of a wallet. Null for items that don't hold an inventory.
        /// </summary>
        public Inventory? GetItemInventory()
        {
            ulong? inventoryId = GetAdditionalData().GetUlong(InventoryKey);
            if (!GetItem().IsItemInventory() || inventoryId == null)
            {
                return null;
            }

            return Manager.GetComponent<Inventory>(InventoryEntities.GetInventoryId((int)inventoryId.Value));
        }

        public uint GetTotalWeight(uint amount)
        {
            uint totalWeight = GetWeight() * amount;

            Inventory? itemInventory = GetItemInventory();
            if (itemInventory != null)
            {
                totalWeight += itemInventory.GetInventoryItems()
                    .Aggregate(0u, (sum, i) => sum + i.GetTotalWeight());
            }

            return totalWeight;
        }

        public uint GetTotalWeight()
        {
            return GetTotalWeight(GetAmount());
        }

        public bool Transfer(Inventory targetInventory, uint amount = 0)
        {
            // Determine how many units to transfer (0 means full stack).
            uint transferAmount = (amount == 0) ? this.GetAmount() : amount;

            // Check if the target inventory has enough available weight to accept the item.
            // Here we use the item's own weight check.
            if (!targetInventory.DoesInventoryItemFit(this, transferAmount))
            {
                return false;
            }

            // Attempt to subtract the transfer amount from the source InventoryItem.
            if (!this.Subtract(transferAmount))
            {
                return false;
            }

            // If subtracting reduced the item count to 0, schedule its deletion.
            if (this.GetAmount() == 0)
            {
                this.ProcessDeletion();
            }

            // Add the item to the target inventory.
            // Pass along the additional data so that any extra properties are preserved.
            bool added = targetInventory.AddItem(this.GetItem(), transferAmount, this.GetAdditionalData());
            if (!added)
            {
                // Roll back the subtraction in case adding to the target inventory fails.
                this.Add(transferAmount);
                return false;
            }

            // Mark that both inventories have changed.
            this.ProcessChanges();
            targetInventory.ProcessChanges();

            return true;
        }
    }
}
