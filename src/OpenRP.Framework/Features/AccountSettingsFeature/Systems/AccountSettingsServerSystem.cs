using OpenRP.Framework.Features.AccountSettingsFeature.Components;
using OpenRP.Framework.Features.AccountSettingsFeature.Services;
using OpenRP.Framework.Shared.ServerEvents.Attributes;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using OpenRP.Framework.Shared.ServerEvents.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.AccountSettingsFeature.Systems
{
    public class AccountSettingsServerSystem : IServerSystem
    {
        [ServerEvent]
        public void OnAccountLoggedIn(OnAccountLoggedInEventArgs args, IAccountSettingsService accountSettingsService)
        {
            // Reload Settings
            AccountSettings settings = accountSettingsService.ReloadAccountSettings(args.Player);
        }
    }
}
