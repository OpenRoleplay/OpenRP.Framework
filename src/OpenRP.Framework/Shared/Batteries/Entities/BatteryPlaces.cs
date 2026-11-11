using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Shared.Batteries.Enums;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;

namespace OpenRP.Framework.Shared.Batteries.Entities
{
    /// <summary>
    /// A snapshot of which inventories belong to which characters, taken once per battery update, so each item's place
    /// is found without scanning every inventory again.
    /// </summary>
    public sealed class BatteryPlaces
    {
        // Guards against a loop in the data (a wallet inside itself).
        private const int MaxInventoryDepth = 32;

        private readonly IReadOnlyDictionary<ulong, ulong> _parentInventoryIds;
        private readonly IReadOnlySet<ulong> _characterInventoryIds;
        private readonly IReadOnlyDictionary<ulong, Player> _playersByInventoryId;

        public BatteryPlaces(IReadOnlyDictionary<ulong, ulong> parentInventoryIds, IReadOnlySet<ulong> characterInventoryIds, IReadOnlyDictionary<ulong, Player> playersByInventoryId)
        {
            _parentInventoryIds = parentInventoryIds;
            _characterInventoryIds = characterInventoryIds;
            _playersByInventoryId = playersByInventoryId;
        }

        public BatteryPlace GetPlace(InventoryItem inventoryItem)
        {
            ulong? topInventoryId = GetTopInventoryId(inventoryItem);
            if (topInventoryId.HasValue && _playersByInventoryId.ContainsKey(topInventoryId.Value))
            {
                return BatteryPlace.WithOnlineCharacter;
            }
            if (topInventoryId.HasValue && _characterInventoryIds.Contains(topInventoryId.Value))
            {
                return BatteryPlace.WithOfflineCharacter;
            }
            return BatteryPlace.Elsewhere;
        }

        /// <summary>
        /// The online player whose character has the item somewhere in their inventory, or null.
        /// </summary>
        public Player? GetHolder(InventoryItem inventoryItem)
        {
            ulong? topInventoryId = GetTopInventoryId(inventoryItem);
            return topInventoryId.HasValue && _playersByInventoryId.TryGetValue(topInventoryId.Value, out Player? player) ? player : null;
        }

        /// <summary>
        /// The inventory the item is in, followed up through the wallets around it to the outermost one.
        /// </summary>
        private ulong? GetTopInventoryId(InventoryItem inventoryItem)
        {
            Inventory? inventory = inventoryItem.GetInventoryIn();
            if (inventory == null)
            {
                return null;
            }

            ulong inventoryId = inventory.GetId();
            for (int depth = 0; depth < MaxInventoryDepth && _parentInventoryIds.TryGetValue(inventoryId, out ulong parentId); depth++)
            {
                inventoryId = parentId;
            }
            return inventoryId;
        }
    }
}
