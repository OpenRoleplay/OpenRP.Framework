using OpenRP.Framework.Features.Permissions.Services;
using OpenRP.Framework.Shared.ServerEvents.Attributes;
using OpenRP.Framework.Shared.ServerEvents.Entities;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Permissions.Systems
{
    public class ServerPermissionSystem : IServerSystem
    {
        [ServerEvent]
        public void OnCharacterSelected(OnCharacterSelectedEventArgs args, IPermissionService permissionManager)
        {
            permissionManager.ReloadCharacterPermissions(args.Character);
        }
    }
}
