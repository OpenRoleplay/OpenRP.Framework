using OpenRP.Framework.Features.Permissions.Services;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Permissions.Systems
{
    public class PermissionSystem : ISystem
    {
        [Event]
        public void OnGameModeInit(IPermissionService permissionManager)
        {
            permissionManager.CreateDatabaseCommandPermissionsIfNotExists();
            permissionManager.RemoveOldCommandPermissions();
        }
    }
}
