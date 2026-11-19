using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// A fishing net fitted to a saved Reefer, with the inventory that holds its catch.
    /// </summary>
    public class VehicleFishingNetModel : BaseModel
    {
        public ulong VehicleId { get; set; }
        public ulong StorageInventoryId { get; set; }

        // Navigational Properties
        public VehicleModel? Vehicle { get; set; }
        public InventoryModel? StorageInventory { get; set; }
    }
}
