using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.DroppedItems.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.DroppedItems.Commands
{
    public class PickupCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Pickup a dropped item from the ground.",
            CommandGroups = new string[] { "Inventory" })]
        public async void Pickup(Player player, IDroppedItemService droppedItemService)
        {
            // DroppedItemService catches and logs its own errors, so nothing is lost in this async void.
            await droppedItemService.PickupNearbyItem(player);
        }
    }
}
