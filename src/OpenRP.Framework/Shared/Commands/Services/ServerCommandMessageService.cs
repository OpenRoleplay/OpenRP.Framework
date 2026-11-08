using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.Chat.Helpers;
using SampSharp.Entities.SAMP;
using SampSharp.Entities.SAMP.Commands;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Shared.Commands.Services
{
    /// <summary>
    /// The messages a player gets when a command can't run: unknown command, no character, no permission, wrong syntax.
    /// </summary>
    public class ServerCommandMessageService : IPlayerCommandMessageService
    {
        public void SendUsage(Player player, IReadOnlyList<CommandDefinition> overloads, string usedCommandName = "")
        {
            player.SendClientMessage(GetUsageMessage(overloads));
        }

        public bool SendPermissionDenied(Player player, CommandDefinition overload)
        {
            if (!player.IsPlayerPlayingAsCharacter())
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You must be playing as a character in order to do this command!");
                return true;
            }

            player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You do not have enough permissions to use this command!");
            return true;
        }

        public bool SendCommandNotFound(Player player, string input)
        {
            player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "This command does not exist!");
            return true;
        }

        private static string GetUsageMessage(IReadOnlyList<CommandDefinition> overloads)
        {
            return overloads.Count == 1
                ? GetCommandSyntax(overloads[0])
                : string.Join(ChatColor.ChromeYellow + " | " + ChatColor.White, overloads.Select(GetCommandSyntax));
        }

        private static string GetCommandSyntax(CommandDefinition command)
        {
            string parameters = string.Join(" ", command.ParsedParameters.Select(GetParameterSyntax));
            string syntax = parameters.Length == 0 ? $"/{command.Name}" : $"/{command.Name} {parameters}";
            return TempChatHelper.ReturnPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, syntax);
        }

        private static string GetParameterSyntax(CommandParameterInfo parameter)
        {
            string name = parameter.Name.Replace("_", " ");
            return parameter.IsRequired ? $"[{name}]" : $"<{name}>";
        }
    }
}
