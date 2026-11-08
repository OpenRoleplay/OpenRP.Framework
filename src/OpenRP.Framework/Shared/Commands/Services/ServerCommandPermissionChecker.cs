using OpenRP.Framework.Features.Permissions.Components;
using OpenRP.Framework.Features.Players.Extensions;
using SampSharp.Entities.SAMP;
using SampSharp.Entities.SAMP.Commands;

namespace OpenRP.Framework.Shared.Commands.Services
{
    /// <summary>
    /// A player may use a command when they play a character that has the command's permission (cmd.&lt;name&gt;).
    /// </summary>
    public class ServerCommandPermissionChecker : IPermissionChecker
    {
        public bool HasPermission(Player player, CommandDefinition command)
        {
            CharacterPermissions? characterPermissions = GetCharacterPermissions(player);
            return characterPermissions != null && characterPermissions.DoesCharacterHavePermission(GetCommandPermission(command));
        }

        /// <summary>
        /// The permission that allows a command, for example cmd.pay for /pay.
        /// </summary>
        public static string GetCommandPermission(CommandDefinition command)
        {
            return $"cmd.{command.Name.ToLower()}";
        }

        private static CharacterPermissions? GetCharacterPermissions(Player player)
        {
            if (!player.IsPlayerPlayingAsCharacter())
            {
                return null;
            }
            return player.GetPlayerCurrentlyPlayingAsCharacter().GetComponent<CharacterPermissions>();
        }
    }
}
