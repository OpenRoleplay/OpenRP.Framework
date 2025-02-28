using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    public class InventoryItemModel : BaseModel
    {
        public ulong ItemId { get; set; }
        public uint Amount { get; set; }
        public uint? UsesRemaining { get; set; }
        public bool KeepOnDeath { get; set; }
        public bool CanDrop { get; set; }
        public bool CanDestroy { get; set; }
        public ulong InventoryId { get; set; }
        public string AdditionalData { get; set; }
        public uint? Weight { get; set; }

        // Navigational Properties
        public DroppedInventoryItemModel? DroppedInventoryItem { get; set; }
        public InventoryModel Inventory { get; set; }
        public ItemModel Item { get; set; }

        // Constructor
        public InventoryItemModel()
        {
            UsesRemaining = null;
            Weight = null;
            DroppedInventoryItem = null;
        }
    }
}