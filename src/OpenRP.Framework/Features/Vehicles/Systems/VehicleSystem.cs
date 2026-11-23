using OpenRP.Framework.Features.Permissions.Services;
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
        public void OnGameModeInit(IServerService serverService, IVehicleManager vehicleManager, IVehicleAccessService vehicleAccessService, IPermissionService permissionService)
        {
            // The admin permission is made before the vehicles spawn; no character is selected yet, so no one needs a reload.
            vehicleAccessService.CreatePermissions(permissionService);
            vehicleManager.LoadAndUnloadVehicles();
        }
    }
}
