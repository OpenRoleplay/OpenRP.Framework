using OpenRP.Framework.Features.Vehicles.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Vehicles.Systems
{
    public class VehicleSystem : ISystem
    {
        [Event]
        public void OnGameModeInit(IServerService serverService, IVehicleManager vehicleManager)
        {
            // VehicleManager
            vehicleManager.LoadAndUnloadVehicles();
        }
    }
}
