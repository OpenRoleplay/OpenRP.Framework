using OpenRP.Framework.Features.Fishing.Components;
using OpenRP.Framework.Shared.BaseManager.Helpers;

namespace OpenRP.Framework.Features.Fishing.Services
{
    public interface IFishingNetManager : IBaseManager
    {
        /// <summary>
        /// The net fitted to this saved vehicle, or null when it has none.
        /// </summary>
        VehicleFishingNet? FindNetOfVehicle(ulong vehicleId);

        /// <summary>
        /// Saves a new net for this saved vehicle right away and loads it.
        /// </summary>
        VehicleFishingNet CreateNet(ulong vehicleId, ulong storageInventoryId);

        /// <summary>
        /// Deletes the net's row right away.
        /// </summary>
        void DeleteNet(VehicleFishingNet net);
    }
}
