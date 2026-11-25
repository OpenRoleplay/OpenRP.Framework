using OpenRP.Framework.Features.Inventories.Components;

namespace OpenRP.Framework.Features.Inventories.Entities
{
    /// <summary>
    /// Gives some items a name of their own in the inventory, built from data another feature owns, for example a bank
    /// card showing its account. Registered with AddInventoryItemNameProvider. Items no provider handles show the name
    /// of their item type.
    /// </summary>
    public interface IInventoryItemNameProvider
    {
        /// <summary>
        /// True when this provider names the item.
        /// </summary>
        bool Handles(InventoryItem inventoryItem);

        /// <summary>
        /// The name shown for the item in the inventory.
        /// </summary>
        string GetName(InventoryItem inventoryItem);

        /// <summary>
        /// Extra text for the item's "Extra Information" column, or null for none.
        /// </summary>
        string? GetExtraInformation(InventoryItem inventoryItem) => null;
    }
}
