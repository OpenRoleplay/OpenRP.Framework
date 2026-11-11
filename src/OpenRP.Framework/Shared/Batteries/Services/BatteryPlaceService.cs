using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Batteries.Entities;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Shared.Batteries.Services
{
    public class BatteryPlaceService : IBatteryPlaceService
    {
        private readonly IEntityManager _entityManager;

        public BatteryPlaceService(IEntityManager entityManager)
        {
            _entityManager = entityManager;
        }

        public BatteryPlaces TakeSnapshot(BaseDataContext dataContext)
        {
            return new BatteryPlaces(GetParentInventoryIds(), GetCharacterInventoryIds(dataContext), GetPlayersByInventoryId());
        }

        /// <summary>
        /// For each inventory inside an item (a wallet), the inventory that item is in.
        /// </summary>
        private Dictionary<ulong, ulong> GetParentInventoryIds()
        {
            Dictionary<ulong, ulong> parentInventoryIds = new Dictionary<ulong, ulong>();
            foreach (InventoryItem inventoryItem in _entityManager.GetComponents<InventoryItem>().Where(inventoryItem => !inventoryItem.IsDeleted() && inventoryItem.GetItem().IsItemInventory()))
            {
                Inventory? innerInventory = inventoryItem.GetItemInventory();
                Inventory? outerInventory = inventoryItem.GetInventoryIn();
                if (innerInventory != null && outerInventory != null)
                {
                    parentInventoryIds[innerInventory.GetId()] = outerInventory.GetId();
                }
            }
            return parentInventoryIds;
        }

        private static HashSet<ulong> GetCharacterInventoryIds(BaseDataContext dataContext)
        {
            return dataContext.Characters.AsNoTracking()
                .Where(character => character.InventoryId != null)
                .Select(character => character.InventoryId!.Value)
                .ToHashSet();
        }

        private Dictionary<ulong, Player> GetPlayersByInventoryId()
        {
            Dictionary<ulong, Player> playersByInventoryId = new Dictionary<ulong, Player>();
            foreach (Player player in _entityManager.GetComponents<Player>().Where(player => player.IsPlayerPlayingAsCharacter()))
            {
                Inventory? inventory = player.GetPlayerCurrentlyPlayingAsCharacter().GetInventory();
                if (inventory != null)
                {
                    playersByInventoryId[inventory.GetId()] = player;
                }
            }
            return playersByInventoryId;
        }
    }
}
