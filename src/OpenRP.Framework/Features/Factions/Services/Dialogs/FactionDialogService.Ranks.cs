using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Factions.Entities;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenRP.Framework.Shared.Dialogs.Extensions;

namespace OpenRP.Framework.Features.Factions.Services.Dialogs
{
    public partial class FactionDialogService
    {
        private void OpenRanks(Player player, Faction faction, Action onBack)
        {
            if (!_factionMembershipService.HasFactionPermission(player.GetPlayerCurrentlyPlayingAsCharacter(), faction, FactionPermissions.ManageRanks))
            {
                ShowMessage(player, "Ranks", "You are no longer allowed to manage ranks in this faction.", onBack);
                return;
            }

            BetterListDialog ranksDialog = new BetterListDialog("Manage", "Back");
            ranksDialog.SetTitle(TitleType.Children, FactionsTitle, faction.GetName(), "Ranks");
            Dictionary<int, FactionRank> ranksByRow = new Dictionary<int, FactionRank>();
            foreach ((FactionRank rank, int depth) in _factionRankService.GetRankTree(faction))
            {
                ranksByRow[ranksDialog.AddRow(GetRankLabel(rank, depth))] = rank;
            }
            int addRankRow = ranksDialog.AddRow($"{ChatColor.CornflowerBlue}Add a rank");

            void OnRankChosen(ListDialogResponse response)
            {
                Action reopenRanks = () => OpenRanks(player, faction, onBack);
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == addRankRow)
                    OpenRankPicker(player, "Add a rank below", GetPossibleParentRanks(player, faction, null), parentRank => OpenAddRank(player, parentRank, reopenRanks), reopenRanks);
                else if (ranksByRow.TryGetValue(response.ItemIndex, out FactionRank? rank))
                    OpenRankActions(player, faction, rank, reopenRanks);
            }

            _dialogService.ShowIgnoringDisconnect(player, ranksDialog, OnRankChosen);
        }

        private void OpenRankActions(Player player, Faction faction, FactionRank rank, Action onBack)
        {
            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            if (!_factionMembershipService.CanManageRank(character, faction, rank))
            {
                ShowMessage(player, rank.GetName(), "You can only manage ranks below your own. You can still add sub-ranks to your own rank with 'Add a rank'.", onBack);
                return;
            }

            BetterListDialog actionsDialog = new BetterListDialog("Choose", "Back");
            actionsDialog.SetTitle(TitleType.Children, FactionsTitle, faction.GetName(), rank.GetName());
            int renameRow = actionsDialog.AddRow("Rename");
            int permissionsRow = actionsDialog.AddRow("Permissions");
            int addSubRankRow = actionsDialog.AddRow("Add a sub-rank");
            int? moveRow = AddRowIf(actionsDialog, !rank.IsTopRank(), "Move under another rank");
            int? deleteRow = AddRowIf(actionsDialog, !rank.IsTopRank(), "Delete");

            void OnActionChosen(ListDialogResponse response)
            {
                Action reopenActions = () => OpenRankActions(player, faction, rank, onBack);
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == renameRow)
                    OpenRenameRank(player, rank, reopenActions);
                else if (response.ItemIndex == permissionsRow)
                    OpenRankPermissions(player, faction, rank, reopenActions);
                else if (response.ItemIndex == addSubRankRow)
                    OpenAddRank(player, rank, reopenActions);
                else if (response.ItemIndex == moveRow)
                    OpenRankPicker(player, $"Move {rank.GetName()} under", GetPossibleParentRanks(player, faction, rank), newParentRank => MoveRank(player, rank, newParentRank, onBack), reopenActions);
                else if (response.ItemIndex == deleteRow)
                    OpenDeleteRank(player, rank, onBack, reopenActions);
            }

            _dialogService.ShowIgnoringDisconnect(player, actionsDialog, OnActionChosen);
        }

        private void OpenRenameRank(Player player, FactionRank rank, Action onBack)
        {
            void Rename(string name)
            {
                string? error = _factionRankService.RenameRank(rank, name);
                ContinueAfter(player, error, onBack, () => OpenRenameRank(player, rank, onBack));
            }

            OpenNameInput(player, $"Rename {rank.GetName()}", $"Enter the new name for {ChatColor.Highlight}{rank.GetName()}{ChatColor.White}:", Rename, onBack);
        }

        private void OpenAddRank(Player player, FactionRank parentRank, Action onBack)
        {
            void Create(string name)
            {
                string? error = _factionRankService.CreateRank(parentRank, name);
                ContinueAfter(player, error, onBack, () => OpenAddRank(player, parentRank, onBack));
            }

            OpenNameInput(player, "Add a rank", $"Enter the name of the new rank below {ChatColor.Highlight}{parentRank.GetName()}{ChatColor.White}:", Create, onBack);
        }

        private void MoveRank(Player player, FactionRank rank, FactionRank newParentRank, Action onDone)
        {
            string? error = _factionRankService.MoveRank(rank, newParentRank);
            ContinueAfter(player, error, onDone, onDone);
        }

        private void OpenDeleteRank(Player player, FactionRank rank, Action onDeleted, Action onBack)
        {
            void Delete()
            {
                string? error = _factionRankService.DeleteRank(rank);
                ContinueAfter(player, error, onDeleted, onBack);
            }

            ShowConfirmation(player, "Delete rank", $"Are you sure you want to delete the rank {ChatColor.Highlight}{rank.GetName()}{ChatColor.White}?", Delete, onBack);
        }

        /// <summary>
        /// The ranks a rank can be added or moved below: the player's own rank and the ranks they manage. When moving,
        /// the rank itself and its sub-ranks are left out.
        /// </summary>
        private List<(FactionRank Rank, int Depth)> GetPossibleParentRanks(Player player, Faction faction, FactionRank? rankToMove)
        {
            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            FactionRank? ownRank = _factionMembershipService.GetMembership(character, faction) is FactionMember membership
                ? _factionRankService.GetRank(membership.GetRankId())
                : null;

            return _factionRankService.GetRankTree(faction)
                .Where(entry => entry.Rank.GetId() == ownRank?.GetId() || _factionMembershipService.CanManageRank(character, faction, entry.Rank))
                .Where(entry => rankToMove == null || (entry.Rank.GetId() != rankToMove.GetId() && !_factionRankService.IsRankBelow(entry.Rank, rankToMove)))
                .ToList();
        }
    }
}
