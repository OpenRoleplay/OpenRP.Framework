using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Database.Services;
using OpenRP.Framework.Features.Inventories.Entities;
using OpenRP.Framework.Features.Inventories.Helpers;
using OpenRP.Framework.Features.Items.Components;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Inventories.Components
{
    public class InventoryItem : Component
    {
        private readonly InventoryItemModel _inventoryItemModel;
        private bool _hasChanges;
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
            EntityId entityId = InventoryEntities.GetInventoryItemId((int)GetId());

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
    }
}
