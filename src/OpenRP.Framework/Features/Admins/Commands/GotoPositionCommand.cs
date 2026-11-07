using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System.Globalization;

namespace OpenRP.Framework.Features.Admins.Commands
{
    public class GotoPositionCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Teleport to a position, for example into a GTA interior.",
            CommandGroups = new string[] { "Admin" })]
        public void GotoPos(Player player, float x, float y, float z, int interior = 0, int virtual_world = 0)
        {
            player.Interior = interior;
            player.VirtualWorld = virtual_world;
            player.Position = new Vector3(x, y, z);
            string position = string.Format(CultureInfo.InvariantCulture, "{0:0.##}, {1:0.##}, {2:0.##}", x, y, z);
            player.SendPlayerInfoMessage(PlayerInfoMessageType.ADMIN, $"Teleported to {position} in interior {interior}, virtual world {virtual_world}.");
        }

        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Teleport to a position, for example into a GTA interior.",
            CommandGroups = new string[] { "Admin" })]
        public void GotoPos(Player player)
        {
            player.SendPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, "/gotopos [x] [y] [z] [interior = 0] [virtual world = 0]");
        }
    }
}
