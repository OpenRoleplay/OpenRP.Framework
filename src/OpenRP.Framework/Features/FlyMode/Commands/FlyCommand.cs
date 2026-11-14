using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.FlyMode.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.FlyMode.Commands
{
    public class FlyCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Fly around freely as a camera; use it again to land where you started.",
            CommandGroups = new string[] { "Admin" })]
        public void Fly(Player player, IFlyModeService flyModeService)
        {
            string? error = flyModeService.ToggleFlying(player);
            if (error != null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
            }
        }
    }
}
