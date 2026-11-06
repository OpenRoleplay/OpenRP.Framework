using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Permissions.Services;
using SampSharp.Entities;
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

        /// <summary>
        /// Faction permissions work right away, instead of at the next character selection.
        /// </summary>
        [ServerEvent]
        public void OnFactionPermissionsChanged(OnFactionPermissionsChangedEventArgs args, IPermissionService permissionManager, IEntityManager entityManager)
        {
            foreach (Character character in entityManager.GetComponents<Character>().Where(character => args.CharacterIds.Contains(character.GetId())).ToList())
            {
                permissionManager.ReloadCharacterPermissions(character);
            }
        }
    }
}
