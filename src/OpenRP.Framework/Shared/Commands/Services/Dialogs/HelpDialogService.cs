using OpenRP.Framework.Features.Commands.Entities;
using OpenRP.Framework.Features.Commands.Helpers;
using OpenRP.Framework.Features.Permissions.Components;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenRP.Framework.Shared.Dialogs.Extensions;

namespace OpenRP.Framework.Shared.Commands.Services.Dialogs
{
    public class HelpDialogService : IHelpDialogService
    {
        private const string RootGroupName = "Root";
        private const string HelpTitle = "Available Commands";

        private readonly IDialogService _dialogService;

        public HelpDialogService(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public void OpenHelp(Player player)
        {
            HashSet<string> availableCommandNames = GetAvailableCommandNames(player);
            if (availableCommandNames.Count == 0)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "No commands available.");
                return;
            }

            OpenCommandGroup(player, ServerCommandGroupCache.CachedServerCommandGroups, availableCommandNames, new List<CommandGroupNode>());
        }

        /// <summary>
        /// The same permissions the command handler checks, so /help never lists a command the player can't run.
        /// </summary>
        private static HashSet<string> GetAvailableCommandNames(Player player)
        {
            if (!player.IsPlayerPlayingAsCharacter())
            {
                return new HashSet<string>();
            }

            CharacterPermissions? characterPermissions = player.GetPlayerCurrentlyPlayingAsCharacter().GetComponent<CharacterPermissions>();
            if (characterPermissions == null)
            {
                return new HashSet<string>();
            }

            return ServerCommandCache.CachedServerCommands
                .Where(command => characterPermissions.DoesPlayerHaveCommandPermission(command.Name))
                .Select(command => command.Name)
                .ToHashSet();
        }

        /// <param name="parentGroups">The groups above this one, from the root down; used for the title and for Back.</param>
        private void OpenCommandGroup(Player player, CommandGroupNode group, HashSet<string> availableCommandNames, List<CommandGroupNode> parentGroups)
        {
            BetterListDialog dialog = new BetterListDialog("Select", "Back");
            dialog.SetTitle(TitleType.Parents, GetGroupTitle(group, parentGroups));

            Dictionary<int, CommandGroupNode> subgroupsByRow = new Dictionary<int, CommandGroupNode>();
            foreach (CommandGroupNode subgroup in GetAvailableSubgroups(group, availableCommandNames))
            {
                subgroupsByRow.Add(dialog.AddRow($"{ChatColor.CornflowerBlue}> {subgroup.Name}"), subgroup);
            }

            Dictionary<int, ServerCommandInfo> commandsByRow = new Dictionary<int, ServerCommandInfo>();
            foreach (ServerCommandInfo command in GetAvailableCommands(group, availableCommandNames))
            {
                commandsByRow.Add(dialog.AddRow($"{ChatColor.White}/{command.Name.ToLower()}"), command);
            }

            _dialogService.ShowIgnoringDisconnect(player, dialog, response =>
            {
                if (response.Response != DialogResponse.LeftButton)
                {
                    OpenParentGroup(player, availableCommandNames, parentGroups);
                }
                else if (subgroupsByRow.TryGetValue(response.ItemIndex, out CommandGroupNode? subgroup))
                {
                    OpenCommandGroup(player, subgroup, availableCommandNames, parentGroups.Append(group).ToList());
                }
                else if (commandsByRow.TryGetValue(response.ItemIndex, out ServerCommandInfo? command))
                {
                    OpenCommandDetails(player, command, group, availableCommandNames, parentGroups);
                }
            });
        }

        /// <summary>
        /// Back from a group: one level up, or close the help at the root.
        /// </summary>
        private void OpenParentGroup(Player player, HashSet<string> availableCommandNames, List<CommandGroupNode> parentGroups)
        {
            if (parentGroups.Count == 0)
            {
                return;
            }

            CommandGroupNode parentGroup = parentGroups[^1];
            OpenCommandGroup(player, parentGroup, availableCommandNames, parentGroups.Take(parentGroups.Count - 1).ToList());
        }

        private void OpenCommandDetails(Player player, ServerCommandInfo command, CommandGroupNode group, HashSet<string> availableCommandNames, List<CommandGroupNode> parentGroups)
        {
            BetterMessageDialog dialog = new BetterMessageDialog("Back", "Close");
            dialog.SetTitle(TitleType.Parents, $"Command: {command.Name}");
            dialog.SetContent(GetCommandDetailsText(command));

            _dialogService.ShowIgnoringDisconnect(player, dialog, response =>
            {
                if (response.Response == DialogResponse.LeftButton)
                {
                    OpenCommandGroup(player, group, availableCommandNames, parentGroups);
                }
            });
        }

        private static string GetCommandDetailsText(ServerCommandInfo command)
        {
            return new StringBuilder()
                .AppendLine($"{ChatColor.CornflowerBlue}Description:{ChatColor.White}")
                .AppendLine(command.Description)
                .ToString();
        }

        private static List<CommandGroupNode> GetAvailableSubgroups(CommandGroupNode group, HashSet<string> availableCommandNames)
        {
            return group.Subgroups
                .Where(subgroup => HasAvailableCommands(subgroup, availableCommandNames))
                .OrderBy(subgroup => subgroup.Name)
                .ToList();
        }

        private static List<ServerCommandInfo> GetAvailableCommands(CommandGroupNode group, HashSet<string> availableCommandNames)
        {
            // A command with overloads appears once per overload in the cache; list it once.
            return group.Commands
                .Where(command => availableCommandNames.Contains(command.Name))
                .DistinctBy(command => command.Name.ToLower())
                .OrderBy(command => command.Name)
                .ToList();
        }

        private static bool HasAvailableCommands(CommandGroupNode group, HashSet<string> availableCommandNames)
        {
            return group.Commands.Any(command => availableCommandNames.Contains(command.Name))
                || group.Subgroups.Any(subgroup => HasAvailableCommands(subgroup, availableCommandNames));
        }

        /// <summary>
        /// "Available Commands -> Admin -> Permissions"-style breadcrumbs.
        /// </summary>
        private static string GetGroupTitle(CommandGroupNode group, List<CommandGroupNode> parentGroups)
        {
            IEnumerable<string> breadcrumbs = parentGroups
                .Append(group)
                .Select(node => node.Name)
                .Where(name => name != RootGroupName)
                .Prepend(HelpTitle);

            return string.Join(ChatColor.CornflowerBlue + " -> " + ChatColor.White, breadcrumbs);
        }
    }
}
