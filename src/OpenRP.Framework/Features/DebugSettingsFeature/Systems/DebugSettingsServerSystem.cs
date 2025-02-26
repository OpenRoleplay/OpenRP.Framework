
using OpenRP.Framework.Shared.ServerEvents.Attributes;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using OpenRP.Framework.Shared.ServerEvents.Entities;
using OpenRP.Framework.Features.DebugSettingsFeature.Services;
using OpenRP.Framework.Features.DebugSettingsFeature.Components;

namespace OpenRP.Framework.Features.DebugSettingsFeature.Systems
{
    public class DebugSettingsServerSystem : IServerSystem
    {
        [ServerEvent]
        public static void OnAccountLoggedIn(OnAccountLoggedInEventArgs args, IDebugSettingsService debugSettingsService)
        {
            DebugSettings settings = debugSettingsService.ReloadDebugSettings(args.Player);
        }
    }
}
