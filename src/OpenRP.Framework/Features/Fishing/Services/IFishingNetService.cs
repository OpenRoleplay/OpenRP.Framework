using OpenRP.Framework.Features.Fishing.Components;
using OpenRP.Framework.Features.Inventories.Components;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Fishing.Services
{
    public interface IFishingNetService
    {
        /// <summary>
        /// Fits the player's Fishing Net to the Reefer they are on or next to, and creates its storage.
        /// </summary>
        void FitNet(Player player);

        /// <summary>
        /// Takes the net off the Reefer the player is on or next to, once its storage is empty.
        /// </summary>
        void RemoveNet(Player player);

        /// <summary>
        /// Deletes the net of a vehicle that was deleted, with its storage and the fish in it.
        /// </summary>
        void RemoveNetOfDeletedVehicle(ulong vehicleId);

        /// <summary>
        /// The Reefer the player sits in, stands on or stands next to, or null when there is none.
        /// </summary>
        Vehicle? FindReeferAtPlayer(Player player);

        /// <summary>
        /// The net fitted to this vehicle, or null when it has none or isn't a saved vehicle.
        /// </summary>
        VehicleFishingNet? GetNet(Vehicle vehicle);

        /// <summary>
        /// The inventory that holds the net's catch, or null when it can't be loaded.
        /// </summary>
        Inventory? GetStorage(VehicleFishingNet net);

        /// <summary>
        /// True when the player's character owns the saved vehicle or holds its key. Admins pass too.
        /// </summary>
        bool CanUseNetOf(Player player, Vehicle vehicle);

        /// <summary>
        /// Why the player can't open this Reefer's net storage, or null when they can.
        /// </summary>
        string? GetNetStorageError(Player player, Vehicle? reefer);

        /// <summary>
        /// True when the player is still on or next to this vehicle.
        /// </summary>
        bool IsPlayerAtVehicle(Player player, Vehicle vehicle);

        /// <summary>
        /// Moves a fish from the net storage to the player's inventory. Returns why it can't, or null when it moved.
        /// </summary>
        string? TakeFishFromNet(Player player, VehicleFishingNet net, InventoryItem fish);

        /// <summary>
        /// Moves a fish from the player's inventory to the net storage. Returns why it can't, or null when it moved.
        /// </summary>
        string? PutFishInNet(Player player, VehicleFishingNet net, InventoryItem fish);
    }
}
