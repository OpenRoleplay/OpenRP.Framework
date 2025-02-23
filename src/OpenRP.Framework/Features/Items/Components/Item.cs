using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Items.Enums;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Items.Components
{
    public class Item : Component
    {
        private ItemModel _itemModel;
        public Item(ItemModel itemModel) 
        { 
            _itemModel = itemModel;
        }

        public ulong GetId()
        {
            return _itemModel.Id;
        }

        public string GetName()
        {
            return _itemModel.Name;
        }

        public ItemModel GetItemModel()
        {
            return _itemModel;
        }

        public uint GetWeight()
        {
            return _itemModel.Weight;
        }

        public bool IsItemWallet()
        {
            if (_itemModel.UseType == ItemType.Wallet)
            {
                return true;
            }
            return false;
        }
        public bool IsItemSkin()
        {
            if (_itemModel.UseType == ItemType.Skin)
            {
                return true;
            }
            return false;
        }
        public bool IsItemAttachment()
        {
            if (_itemModel.UseType == ItemType.Attachment)
            {
                return true;
            }
            return false;
        }

        public bool IsItemCurrency()
        {
            if (_itemModel.UseType == ItemType.Currency)
            {
                return true;
            }
            return false;
        }

        public bool IsItemVehicleKey()
        {
            if (_itemModel.UseType == ItemType.VehicleKey)
            {
                return true;
            }
            return false;
        }

        public bool IsItemInventory()
        {
            return IsItemWallet();
        }
    }
}
