using OpenRP.Framework.Features.Inventories.Components;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs
{
    /// <summary>
    /// A player chose "Use" on an item in an inventory. Features react to their own item types, for example a radio
    /// opens its menu. An item with a use limit may already be used up and deleted.
    /// </summary>
    public class OnPlayerInventoryItemUsedEventArgs : ServerEventArgs
    {
        public required Player Player { get; init; }
        public required InventoryItem InventoryItem { get; init; }
    }
}
