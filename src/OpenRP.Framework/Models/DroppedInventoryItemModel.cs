namespace OpenRP.Framework.Models
{
    public class DroppedInventoryItemModel
    {
        public ulong Id { get; set; }
        public ulong InventoryItemId { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
        public float? RotX { get; set; } = null;
        public float? RotY { get; set; } = null;
        public float? RotZ { get; set; } = null;

        // Navigational Properties
        public InventoryItemModel InventoryItem { get; set; }
    }
}