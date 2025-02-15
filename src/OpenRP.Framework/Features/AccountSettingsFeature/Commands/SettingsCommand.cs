using OpenRP.Framework.Features.Commands.Attributes;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Features.AccountSettingsFeature.Services;

namespace OpenRP.Framework.Features.AccountSettingsFeature.Commands
{
    public class SettingsCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Change your account settings that are shared across all of your characters.",
            CommandGroups = new string[] { "Account" })]
        public void Settings(Player player, IAccountSettingsService accountSettingsService)
        {
            accountSettingsService.OpenAccountSettingsDialog(player);
        }
    }
}
