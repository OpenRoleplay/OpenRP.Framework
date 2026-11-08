using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using SampSharp.Entities.SAMP;
using System;

namespace OpenRP.Framework.Features.Factions.Services.Dialogs
{
    public partial class FactionDialogService
    {
        public void OpenInvitation(Player player)
        {
            PlayerFactionInvitation? invitation = player.GetComponent<PlayerFactionInvitation>();
            Faction? faction = invitation == null ? null : _factionService.GetFaction(invitation.GetFactionId());
            FactionRank? rank = invitation == null ? null : _factionRankService.GetRank(invitation.GetRankId());
            if (invitation == null || faction == null || rank == null)
            {
                return;
            }

            BetterMessageDialog invitationDialog = new BetterMessageDialog("Accept", "Decline");
            invitationDialog.SetTitle(TitleType.Children, FactionsTitle, "Invitation");
            invitationDialog.SetContent($"{ChatColor.Highlight}{invitation.GetInviterName()}{ChatColor.White} invites you to join {ChatColor.Highlight}{faction.GetName()}{ChatColor.White} as {ChatColor.Highlight}{rank.GetName()}{ChatColor.White}.");

            void OnInvitationAnswered(MessageDialogResponse response)
            {
                if (response.Response != DialogResponse.LeftButton)
                {
                    _factionMembershipService.DeclineInvitation(player);
                    player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"You declined the invitation to {faction.GetName()}.");
                    return;
                }

                string? error = _factionMembershipService.AcceptInvitation(player);
                player.SendPlayerInfoMessage(error == null ? PlayerInfoMessageType.INFO : PlayerInfoMessageType.ERROR, error ?? $"You are now a member of {faction.GetName()}. Use /faction to open its menu.");
            }

            _dialogService.Show(player, invitationDialog, OnInvitationAnswered);
        }

        private void OpenInvitePlayer(Player player, Faction faction, Action onBack)
        {
            void FindInvitee(string input)
            {
                Player? invitee = int.TryParse(input.Trim(), out int playerId) ? _entityManager.GetComponents<Player>().FirstOrDefault(candidate => candidate.Id == playerId) : null;
                if (invitee == null)
                {
                    ShowMessage(player, "Invite a player", $"There is no player with the id '{input}'.", () => OpenInvitePlayer(player, faction, onBack));
                    return;
                }

                OpenRankPicker(player, $"Invite {invitee.Name} as", _factionMembershipService.GetManageableRanks(player.GetPlayerCurrentlyPlayingAsCharacter(), faction), rank => Invite(player, invitee, faction, rank, onBack), onBack);
            }

            OpenNameInput(player, "Invite a player", "Enter the id of the player you want to invite:", FindInvitee, onBack);
        }

        private void Invite(Player player, Player invitee, Faction faction, FactionRank rank, Action onBack)
        {
            string? error = _factionMembershipService.Invite(player.GetPlayerCurrentlyPlayingAsCharacter(), invitee, faction, rank);
            if (error != null)
            {
                ShowMessage(player, "Invite a player", error, onBack);
                return;
            }

            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"You invited {invitee.Name} to {faction.GetName()} as {rank.GetName()}.");
            OpenInvitation(invitee);
        }
    }
}
