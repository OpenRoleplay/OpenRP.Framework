using OpenRP.Framework.Features.Inventories.Components;
using SampSharp.Entities.SAMP;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.DroppedItems.Services
{
    public interface IDroppedItemService
    {
        /// <summary>
        /// Puts every dropped item from the database back on the ground. Called once at startup.
        /// </summary>
        void LoadDroppedItems();

        /// <summary>
        /// Drops this amount of the inventory item on the ground below the player. False when it can't be dropped.
        /// </summary>
        Task<bool> DropItem(Player player, InventoryItem inventoryItem, uint amount);

        /// <summary>
        /// Picks up the dropped item the player is standing next to, if it fits in their inventory.
        /// </summary>
        Task PickupNearbyItem(Player player);
    }
}
