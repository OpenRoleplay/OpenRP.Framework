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

namespace OpenRP.Framework.Features.Factions.Services.Dialogs
{
    public partial class FactionDialogService
    {
        private void OpenMembers(Player player, Faction faction, Action onBack)
        {
            List<FactionMember> members = _factionMembershipService.GetMembers(faction);
            Dictionary<ulong, string> names = _characterService.GetCharacterNames(members.Select(member => member.GetCharacterId()));
            HashSet<ulong> onlineCharacterIds = _entityManager.GetComponents<Character>().Select(character => character.GetId()).ToHashSet();

            BetterTablistDialog membersDialog = new BetterTablistDialog("Manage", "Back", 3);
            membersDialog.SetTitle(TitleType.Children, FactionsTitle, faction.GetName(), "Members");
            membersDialog.AddHeaders("Name", "Rank", "Status");
            Dictionary<int, FactionMember> membersByRow = new Dictionary<int, FactionMember>();
            foreach (FactionMember member in members.OrderBy(member => GetMemberName(names, member)))
            {
                string status = onlineCharacterIds.Contains(member.GetCharacterId()) ? $"{ChatColor.DarkGreen}Online" : "Offline";
                string rankName = _factionRankService.GetRank(member.GetRankId())?.GetName() ?? "-";
                membersByRow[membersDialog.AddRow(GetMemberName(names, member), rankName, status)] = member;
            }

            void OnMemberChosen(TablistDialogResponse response)
            {
                if (response.Response != DialogResponse.LeftButton || !membersByRow.TryGetValue(response.ItemIndex, out FactionMember? member))
                {
                    onBack();
                    return;
                }
                OpenMemberActions(player, faction, member, GetMemberName(names, member), () => OpenMembers(player, faction, onBack));
            }

            _dialogService.Show(player, membersDialog, OnMemberChosen);
        }

        private void OpenMemberActions(Player player, Faction faction, FactionMember member, string memberName, Action onBack)
        {
            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            FactionRank? memberRank = _factionRankService.GetRank(member.GetRankId());
            bool canManageMember = memberRank != null && _factionMembershipService.CanManageRank(character, faction, memberRank);

            BetterListDialog actionsDialog = new BetterListDialog("Choose", "Back");
            actionsDialog.SetTitle(TitleType.Children, FactionsTitle, faction.GetName(), memberName);
            int? changeRankRow = AddRowIf(actionsDialog, canManageMember && _factionMembershipService.HasFactionPermission(character, faction, FactionPermissions.SetRank), "Change rank");
            int? removeRow = AddRowIf(actionsDialog, canManageMember && _factionMembershipService.HasFactionPermission(character, faction, FactionPermissions.Remove), "Remove from faction");

            if (changeRankRow == null && removeRow == null)
            {
                ShowMessage(player, memberName, $"{memberName} is {ChatColor.Highlight}{memberRank?.GetName() ?? "-"}{ChatColor.White}. You can't manage this member.", onBack);
                return;
            }

            void OnActionChosen(ListDialogResponse response)
            {
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == changeRankRow)
                    OpenChangeMemberRank(player, faction, member, memberName, onBack);
                else if (response.ItemIndex == removeRow)
                    OpenRemoveMember(player, member, memberName, onBack);
            }

            _dialogService.Show(player, actionsDialog, OnActionChosen);
        }

        private void OpenChangeMemberRank(Player player, Faction faction, FactionMember member, string memberName, Action onBack)
        {
            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();

            void ChangeRank(FactionRank newRank)
            {
                string? error = _factionMembershipService.SetMemberRank(character, member, newRank);
                ContinueAfter(player, error, onBack, onBack);
            }

            OpenRankPicker(player, $"New rank for {memberName}", _factionMembershipService.GetManageableRanks(character, faction), ChangeRank, onBack);
        }

        private void OpenRemoveMember(Player player, FactionMember member, string memberName, Action onBack)
        {
            void Remove()
            {
                string? error = _factionMembershipService.RemoveMember(player.GetPlayerCurrentlyPlayingAsCharacter(), member);
                ContinueAfter(player, error, onBack, onBack);
            }

            ShowConfirmation(player, "Remove member", $"Are you sure you want to remove {memberName} from the faction?", Remove, onBack);
        }

        private static string GetMemberName(Dictionary<ulong, string> names, FactionMember member)
        {
            return names.TryGetValue(member.GetCharacterId(), out string? name) ? name : $"Character {member.GetCharacterId()}";
        }
    }
}
