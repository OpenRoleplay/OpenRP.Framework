using OpenRP.Framework.Features.Characters.Components;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Vehicles.Services
{
    public interface IVehicleKeyService
    {
        /// <summary>
        /// True when the character carries a key for this vehicle in their inventory.
        /// </summary>
        bool HasVehicleKey(Character character, ulong vehicleId);

        /// <summary>
        /// Adds a key for this vehicle to the player's character inventory. Returns why it was refused, or null when the key was given.
        /// </summary>
        string? GiveVehicleKey(Player player, ulong vehicleId);

        /// <summary>
        /// Deletes every key to this vehicle, in every inventory, when the vehicle is deleted.
        /// </summary>
        void RemoveVehicleKeys(ulong vehicleId);
    }
}
