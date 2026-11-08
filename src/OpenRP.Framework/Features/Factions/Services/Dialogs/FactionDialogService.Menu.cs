using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Factions.Entities;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Factions.Services.Dialogs
{
    public partial class FactionDialogService
    {
        public void OpenFactions(Player player)
        {
            if (!player.IsPlayerPlayingAsCharacter())
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You must be playing as a character in order to do this command!");
                return;
            }

            List<Faction> factions = _factionMembershipService.GetManageableFactions(player.GetPlayerCurrentlyPlayingAsCharacter());
            if (factions.Count == 0)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You are not a member of any faction.");
                return;
            }

            if (factions.Count == 1)
            {
                OpenFactionMenu(player, factions[0], () => { });
                return;
            }

            OpenFactionChoice(player, factions, "Choose a faction", () => { });
        }

        private void OpenFactionChoice(Player player, List<Faction> factions, string title, Action onBack)
        {
            BetterListDialog factionDialog = new BetterListDialog("Open", "Back");
            factionDialog.SetTitle(TitleType.Children, FactionsTitle, title);
            Dictionary<int, Faction> factionsByRow = new Dictionary<int, Faction>();
            foreach (Faction faction in factions)
            {
                factionsByRow[factionDialog.AddRow(faction.GetName())] = faction;
            }

            void OnFactionChosen(ListDialogResponse response)
            {
                if (response.Response != DialogResponse.LeftButton || !factionsByRow.TryGetValue(response.ItemIndex, out Faction? faction))
                {
                    onBack();
                    return;
                }
                OpenFactionMenu(player, faction, () => OpenFactionChoice(player, factions, title, onBack));
            }

            _dialogService.Show(player, factionDialog, OnFactionChosen);
        }

        private void OpenFactionMenu(Player player, Faction faction, Action onBack)
        {
            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            BetterListDialog menuDialog = new BetterListDialog("Open", "Back");
            menuDialog.SetTitle(TitleType.Children, FactionsTitle, faction.GetName());

            int membersRow = menuDialog.AddRow($"Members ({_factionMembershipService.GetMembers(faction).Count})");
            int? ranksRow = AddRowIf(menuDialog, _factionMembershipService.HasFactionPermission(character, faction, FactionPermissions.ManageRanks), "Ranks");
            int? inviteRow = AddRowIf(menuDialog, _factionMembershipService.HasFactionPermission(character, faction, FactionPermissions.Invite), "Invite a player");
            List<Faction> subFactions = _factionService.GetChildFactions(faction);
            int? subFactionsRow = AddRowIf(menuDialog, subFactions.Count > 0 && _factionMembershipService.IsLeader(character, faction), $"Sub-factions ({subFactions.Count})");
            int? permissionsRow = AddRowIf(menuDialog, _factionMembershipService.IsLeader(character, faction), "Permissions");
            FactionMember? membership = _factionMembershipService.GetMembership(character, faction);
            bool canSetActive = membership != null && _factionMembershipService.GetMemberships(character).Count > 1 && !membership.IsActive();
            int? setActiveRow = AddRowIf(menuDialog, canSetActive, "Set as active faction");
            int? leaveRow = AddRowIf(menuDialog, membership != null, "Leave faction");

            void OnMenuItemChosen(ListDialogResponse response)
            {
                Action reopenMenu = () => OpenFactionMenu(player, faction, onBack);
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == permissionsRow)
                    OpenFactionPermissions(player, faction, reopenMenu);
                else if (response.ItemIndex == setActiveRow)
                    SetActiveFaction(player, faction, reopenMenu);
                else if (response.ItemIndex == membersRow)
                    OpenMembers(player, faction, reopenMenu);
                else if (response.ItemIndex == ranksRow)
                    OpenRanks(player, faction, reopenMenu);
                else if (response.ItemIndex == inviteRow)
                    OpenInvitePlayer(player, faction, reopenMenu);
                else if (response.ItemIndex == subFactionsRow)
                    OpenFactionChoice(player, subFactions, $"{faction.GetName()} sub-factions", reopenMenu);
                else if (response.ItemIndex == leaveRow)
                    OpenLeaveFaction(player, faction, reopenMenu);
            }

            _dialogService.Show(player, menuDialog, OnMenuItemChosen);
        }

        private void SetActiveFaction(Player player, Faction faction, Action onDone)
        {
            string? error = _factionMembershipService.SetActiveFaction(player.GetPlayerCurrentlyPlayingAsCharacter(), faction);
            ContinueAfter(player, error, () =>
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"{faction.GetName()} is now your active faction. /fooc goes to its members.");
                onDone();
            }, onDone);
        }

        private void OpenLeaveFaction(Player player, Faction faction, Action onBack)
        {
            void Leave()
            {
                string? error = _factionMembershipService.Leave(player.GetPlayerCurrentlyPlayingAsCharacter(), faction);
                ContinueAfter(player, error, () => player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"You have left {faction.GetName()}."), onBack);
            }

            ShowConfirmation(player, "Leave faction", $"Are you sure you want to leave {faction.GetName()}?", Leave, onBack);
        }

        private static int? AddRowIf(BetterListDialog dialog, bool condition, string text)
        {
            return condition ? dialog.AddRow(text) : null;
        }
    }
}
