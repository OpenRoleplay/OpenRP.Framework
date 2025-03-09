using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Database;
using OpenRP.Framework.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Inventories.Entities;
using OpenRP.Framework.Shared.BaseManager.Entities;

namespace OpenRP.Framework.Features.Characters.Components
{
    public class Character : Component, IBaseDataComponent
    {
        private Player? _player;
        private CharacterModel _characterModel;

        public Character(CharacterModel characterModel)
        {
            _characterModel = characterModel;
        }

        public ulong GetId()
        {
            return GetCharacterModel().Id;
        }

        public Player? GetPlayer()
        {
            if (_player == null && this.Entity.IsOfType(SampEntities.PlayerType))
            {
                _player = this.GetComponent<Player>();
            }
            return _player;
        }

        public Inventory? GetInventory()
        {
            if(_characterModel.InventoryId != null)
            {
                EntityId entityId = InventoryEntities.GetInventoryId((int)_characterModel.InventoryId);

                return Manager.GetComponent<Inventory>(entityId);
            }
            return null;
        }

        public Inventory? GetWallet()
        {
            Inventory? inventoryToCheck = GetInventory();
            if(inventoryToCheck != null)
            {
                foreach(InventoryItem inventoryItem in inventoryToCheck.GetInventoryItems())
                {
                    if(inventoryItem.GetItem().IsItemWallet() && inventoryItem.GetAdditionalData().GetBoolean("DEEFAULT_WALLET") == true)
                    {
                        return inventoryItem.GetItemInventory();
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Method to get the Character Model. Don't use, will be deprecated in the future.
        /// </summary>
        public CharacterModel GetCharacterModel()
        {
            return _characterModel;
        }

        public string GetName()
        {
            return String.Format("{0} {1}", _characterModel.FirstName, _characterModel.LastName);
        }

        public bool HasAccent()
        {
            return !String.IsNullOrEmpty(_characterModel.Accent);
        }

        public string? GetAccent()
        {
            if (HasAccent())
            {
                return _characterModel.Accent;
            }
            return null;
        }
    }
}
