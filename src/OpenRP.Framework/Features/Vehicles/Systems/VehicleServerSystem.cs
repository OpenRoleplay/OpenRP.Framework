using OpenRP.Framework.Features.Vehicles.Services;
using OpenRP.Framework.Shared.ServerEvents.Attributes;
using OpenRP.Framework.Shared.ServerEvents.Entities;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;

namespace OpenRP.Framework.Features.Vehicles.Systems
{
    public class VehicleServerSystem : IServerSystem
    {
        /// <summary>
        /// Vehicles may have been created before the jurisdictions were loaded, with an SA plate for all of them.
        /// </summary>
        [ServerEvent]
        public void OnJurisdictionsLoaded(OnJurisdictionsLoadedEventArgs args, IVehicleManager vehicleManager)
        {
            vehicleManager.RefreshNumberPlates();
        }

        [ServerEvent]
        public void OnJurisdictionPlateTagChanged(OnJurisdictionPlateTagChangedEventArgs args, IVehicleManager vehicleManager)
        {
            vehicleManager.RefreshNumberPlates();
        }

        [ServerEvent]
        public void OnJurisdictionDeleted(OnJurisdictionDeletedEventArgs args, IVehicleManager vehicleManager)
        {
            vehicleManager.RemoveVehiclesFromJurisdiction(args.JurisdictionId);
        }
    }
}
