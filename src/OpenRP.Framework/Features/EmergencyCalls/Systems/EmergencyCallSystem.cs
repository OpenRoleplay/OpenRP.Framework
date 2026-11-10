using OpenRP.Framework.Features.EmergencyCalls.Entities;
using OpenRP.Framework.Features.Permissions.Services;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.EmergencyCalls.Systems
{
    public class EmergencyCallSystem : ISystem
    {
        /// <summary>
        /// Creates dispatch.receive, so admins can grant it to a faction with /factionpermission.
        /// </summary>
        [Event]
        public void OnGameModeInit(IPermissionService permissionService)
        {
            permissionService.CreatePermissionIfNotExists(DispatchPermissions.Receive, DispatchPermissions.ReceiveDescription);
        }
    }
}
