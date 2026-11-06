using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Inventories.Components;
using System.Linq;

namespace OpenRP.Framework.Features.Vehicles.Services
{
    public class VehicleKeyService : IVehicleKeyService
    {
        public bool HasVehicleKey(Character character, ulong vehicleId)
        {
            Inventory? inventory = character.GetInventory();
            if (inventory == null)
            {
                return false;
            }

            return inventory.GetInventoryItems().Any(inventoryItem => inventoryItem.GetVehicleId() == vehicleId);
        }
    }
}
