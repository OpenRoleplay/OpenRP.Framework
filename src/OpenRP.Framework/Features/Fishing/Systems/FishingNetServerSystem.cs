using OpenRP.Framework.Features.Fishing.Services;
using OpenRP.Framework.Shared.ServerEvents.Attributes;
using OpenRP.Framework.Shared.ServerEvents.Entities;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;

namespace OpenRP.Framework.Features.Fishing.Systems
{
    public class FishingNetServerSystem : IServerSystem
    {
        /// <summary>
        /// The net's row went with the vehicle (cascade), but its storage and the fish in it did not.
        /// </summary>
        [ServerEvent]
        public void OnVehicleDeleted(OnVehicleDeletedEventArgs args, IFishingNetService fishingNetService)
        {
            fishingNetService.RemoveNetOfDeletedVehicle(args.VehicleId);
        }
    }
}
