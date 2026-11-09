using OpenRP.Framework.Database.Models;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Vehicles.Services
{
    public interface IVehicleManager
    {
        void LoadAndUnloadVehicles();
        Vehicle CreateVehicle(VehicleModel vehicle);
        void UpdateVehicle(Vehicle vehicle);
        void UpdateVehicle(Vehicle vehicle, VehicleModel vehicleData);

        /// <summary>
        /// Sets the plate of every loaded vehicle again, for when the tags of jurisdictions changed.
        /// </summary>
        void RefreshNumberPlates();

        /// <summary>
        /// Unregisters the loaded vehicles of a jurisdiction that was deleted, so they get an SA plate.
        /// </summary>
        void RemoveVehiclesFromJurisdiction(ulong jurisdictionId);
    }
}
