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

        public Inventory GetItemInventory()
        {
            List<Inventory> allInventories = Manager.GetComponents<Inventory>().ToList();

            return allInventories.SingleOrDefault(inv =>
                inv.GetInventoryItems().Any(item =>
                {
                    Item itemInstance = item.GetItem();
                    // Check that this item is an inventory item and that its AdditionalData contains a reference to the given inventory's ID.
                    return itemInstance.IsItemInventory() &&
                        GetAdditionalData().GetString("INVENTORY") == inv.GetId().ToString();
                }));
        }

        public uint GetTotalWeight(uint amount)
        {
            uint totalWeight = GetWeight() * amount;

            if (GetItem().IsItemInventory())
            {
                totalWeight += GetItemInventory().GetInventoryItems()
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
