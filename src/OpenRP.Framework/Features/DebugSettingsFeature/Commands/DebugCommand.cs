using OpenRP.Framework.Features.Commands.Attributes;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Features.DebugSettingsFeature.Services;

namespace OpenRP.Framework.Features.DebugSettingsFeature.Commands
{
    public class DebugCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Tester" },
            Description = "Open the debug settings dialog.",
            CommandGroups = new string[] { "Debug" } )]
        public void Debug(Player player, IDebugSettingsService debugSettingsService)
        {
            debugSettingsService.OpenDebugSettingsDialog(player);
        }
    }
}
