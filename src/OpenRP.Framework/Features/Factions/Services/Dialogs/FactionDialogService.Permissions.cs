using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Factions.Components;
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
        private const string InactiveStatus = ChatColor.WarmYellow + "Inactive";

        /// <summary>
        /// What the faction may do. Leaders of the parent faction can grant from the parent's own permissions here;
        /// everyone else only sees the list.
        /// </summary>
        private void OpenFactionPermissions(Player player, Faction faction, Action onBack)
        {
            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            Faction? parent = _factionService.GetParentFaction(faction);
            bool canGrant = parent != null && _factionMembershipService.IsLeader(character, parent);

            IEnumerable<string> shownPermissions = canGrant ? _factionPermissionService.GetFactionPermissions(parent!) : _factionPermissionService.GetFactionPermissions(faction);
            List<string> permissions = shownPermissions.Union(_factionPermissionService.GetInactiveFactionGrants(faction)).OrderBy(permission => permission).ToList();

            BetterTablistDialog permissionsDialog = new BetterTablistDialog(canGrant ? "Toggle" : "Back", canGrant ? "Back" : null!, 2);
            permissionsDialog.SetTitle(TitleType.Children, FactionsTitle, faction.GetName(), "Permissions");
            permissionsDialog.AddHeaders("Permission", "Status");
            Dictionary<int, string> permissionsByRow = new Dictionary<int, string>();
            foreach (string permission in permissions)
            {
                permissionsByRow[permissionsDialog.AddRow(GetPermissionLabel(permission), GetFactionPermissionStatus(faction, permission))] = permission;
            }

            void OnPermissionChosen(TablistDialogResponse response)
            {
                if (!canGrant || response.Response != DialogResponse.LeftButton || !permissionsByRow.TryGetValue(response.ItemIndex, out string? permission))
                {
                    onBack();
                    return;
                }
                ToggleSubFactionPermission(player, faction, permission, () => OpenFactionPermissions(player, faction, onBack));
            }

            _dialogService.ShowIgnoringDisconnect(player, permissionsDialog, OnPermissionChosen);
        }

        private void ToggleSubFactionPermission(Player player, Faction faction, string permission, Action onDone)
        {
            if (_factionPermissionService.IsDefaultPermission(permission))
            {
                ShowMessage(player, "Permissions", "Every faction has this permission.", onDone);
                return;
            }

            bool grant = !_factionPermissionService.IsGrantedToFaction(faction, permission);
            string? error = _factionMembershipService.SetSubFactionPermission(player.GetPlayerCurrentlyPlayingAsCharacter(), faction, permission, grant);
            ContinueAfter(player, error, onDone, onDone);
        }

        /// <summary>
        /// Every permission the rank's faction has, with whether the rank has it. Only permissions the player has can be
        /// toggled; grants the faction no longer has are shown as inactive.
        /// </summary>
        private void OpenRankPermissions(Player player, Faction faction, FactionRank rank, Action onBack)
        {
            List<string> permissions = _factionPermissionService.GetFactionPermissions(faction)
                .Union(_factionPermissionService.GetInactiveRankGrants(rank))
                .OrderBy(permission => permission)
                .ToList();

            BetterTablistDialog permissionsDialog = new BetterTablistDialog("Toggle", "Back", 2);
            permissionsDialog.SetTitle(TitleType.Children, FactionsTitle, rank.GetName(), "Permissions");
            permissionsDialog.AddHeaders("Permission", "Granted");
            Dictionary<int, string> permissionsByRow = new Dictionary<int, string>();
            foreach (string permission in permissions)
            {
                permissionsByRow[permissionsDialog.AddRow(GetPermissionLabel(permission), GetRankPermissionStatus(rank, permission))] = permission;
            }

            void OnPermissionChosen(TablistDialogResponse response)
            {
                Action reopenPermissions = () => OpenRankPermissions(player, faction, rank, onBack);
                if (response.Response != DialogResponse.LeftButton || !permissionsByRow.TryGetValue(response.ItemIndex, out string? permission))
                {
                    onBack();
                    return;
                }

                bool grant = !_factionPermissionService.IsGrantedToRank(rank, permission);
                string? error = _factionMembershipService.SetRankPermission(player.GetPlayerCurrentlyPlayingAsCharacter(), faction, rank, permission, grant);
                ContinueAfter(player, error, reopenPermissions, reopenPermissions);
            }

            _dialogService.ShowIgnoringDisconnect(player, permissionsDialog, OnPermissionChosen);
        }

        private string GetFactionPermissionStatus(Faction faction, string permission)
        {
            if (_factionPermissionService.IsDefaultPermission(permission))
                return "Default";
            if (!_factionPermissionService.IsGrantedToFaction(faction, permission))
                return $"{ChatColor.Red}Not granted";
            return _factionPermissionService.FactionHasPermission(faction, permission) ? $"{ChatColor.DarkGreen}Granted" : InactiveStatus;
        }

        private string GetRankPermissionStatus(FactionRank rank, string permission)
        {
            if (rank.IsTopRank() || _factionPermissionService.RankHasPermission(rank, permission))
                return $"{ChatColor.DarkGreen}Yes";
            return _factionPermissionService.IsGrantedToRank(rank, permission) ? InactiveStatus : $"{ChatColor.Red}No";
        }

        private string GetPermissionLabel(string permission)
        {
            return _factionPermissionService.GetPermissionDescription(permission);
        }
    }
}
