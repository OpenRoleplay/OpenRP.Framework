using OpenRP.Framework.Features.Admins.Helpers;
using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System.Globalization;

namespace OpenRP.Framework.Features.Admins.Commands
{
    public class TeleportRelativeCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Move a player a distance forward, back, left, right, up or down, keeping their interior and virtual world.",
            CommandGroups = new string[] { "Admin" })]
        public void TpRel(Player player, Player target, float distance, string direction)
        {
            if (!RelativeDirections.TryParse(direction, out RelativeDirection relativeDirection))
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, $"'{direction}' is not a direction. Use {RelativeDirections.Names}.");
                return;
            }

            MoveTarget(target, relativeDirection, distance);
            SendTeleportedMessages(player, target, distance, relativeDirection);
        }

        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Move a player a distance forward, back, left, right, up or down, keeping their interior and virtual world.",
            CommandGroups = new string[] { "Admin" })]
        public void TpRel(Player player)
        {
            player.SendPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, $"/tprel [playerid] [distance] [direction], for example /tprel {player.Id} 10 forward");
            player.SendPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, $"Directions: {RelativeDirections.Names}.");
        }

        /// <summary>
        /// Moves only the position. In a vehicle the vehicle moves, with everyone in it: setting a player's position
        /// would throw them out of it.
        /// </summary>
        private static void MoveTarget(Player target, RelativeDirection direction, float distance)
        {
            Vehicle? vehicle = target.Vehicle;
            if (vehicle != null)
            {
                vehicle.Position += RelativeDirections.GetOffset(direction, vehicle.Angle, distance);
                return;
            }

            target.Position += RelativeDirections.GetOffset(direction, target.Angle, distance);
        }

        private static void SendTeleportedMessages(Player player, Player target, float distance, RelativeDirection direction)
        {
            string movement = string.Format(CultureInfo.InvariantCulture, "{0:0.##} {1}", distance, direction.ToString().ToLowerInvariant());
            player.SendPlayerInfoMessage(PlayerInfoMessageType.ADMIN, $"Teleported {target.Name} {movement}.");

            if (target != player)
            {
                target.SendPlayerInfoMessage(PlayerInfoMessageType.ADMIN, $"An admin teleported you {movement}.");
            }
        }
    }
}
