
using OpenRP.Framework.Shared.ServerEvents.Attributes;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using OpenRP.Framework.Shared.ServerEvents.Entities;
using OpenRP.Framework.Features.DebugSettingsFeature.Services;
using OpenRP.Framework.Features.DebugSettingsFeature.Components;
using SampSharp.Entities.SAMP;
using OpenRP.Framework.Features.Accounts.Components;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Permissions.Components;

namespace OpenRP.Framework.Features.DebugSettingsFeature.Systems
{
    public class DebugSettingsServerSystem : IServerSystem
    {
        [ServerEvent]
        public static void OnCharacterSelectedEventArgs(OnCharacterSelectedEventArgs args, IDebugSettingsService debugSettingsService)
        {
            Character character = args.Character;

            if (character != null)
            {
                CharacterPermissions characterPermissions = character.GetComponent<CharacterPermissions>();

                if (characterPermissions != null && characterPermissions.DoesPlayerHaveCommandPermission("cmd.debug"))
                {
                    debugSettingsService.ReloadDebugSettings(args.Player);
                }
            }
        }
    }
}
