using OpenRP.Framework.Features.Characters.Components;

namespace OpenRP.Framework.Features.Vehicles.Services
{
    public interface IVehicleKeyService
    {
        /// <summary>
        /// True when the character carries a key for this vehicle in their inventory.
        /// </summary>
        bool HasVehicleKey(Character character, ulong vehicleId);
    }
}
