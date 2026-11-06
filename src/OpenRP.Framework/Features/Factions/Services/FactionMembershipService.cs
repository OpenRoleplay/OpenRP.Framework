using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Factions.Entities;
using OpenRP.Framework.Features.Players.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Factions.Services
{
    public class FactionMembershipService : IFactionMembershipService
    {
        private const string TopRankName = "Leader";
        private const string LastLeaderError = "The last leader can't do this; appoint another leader first.";

        private readonly IEntityManager _entityManager;
        private readonly IFactionService _factionService;
        private readonly IFactionRankService _factionRankService;
        private readonly IFactionManager _factionManager;
        private readonly IFactionMemberManager _factionMemberManager;
        private readonly IFactionPermissionService _factionPermissionService;

        public FactionMembershipService(IEntityManager entityManager, IFactionService factionService, IFactionRankService factionRankService, IFactionManager factionManager, IFactionMemberManager factionMemberManager, IFactionPermissionService factionPermissionService)
        {
            _entityManager = entityManager;
            _factionService = factionService;
            _factionRankService = factionRankService;
            _factionManager = factionManager;
            _factionMemberManager = factionMemberManager;
            _factionPermissionService = factionPermissionService;
        }

        public List<FactionMember> GetMemberships(Character character)
        {
            return GetLoadedMemberships().Where(member => member.GetCharacterId() == character.GetId()).ToList();
        }

        public FactionMember? GetMembership(Character character, Faction faction)
        {
            return GetMemberships(character).FirstOrDefault(member => member.GetFactionId() == faction.GetId());
        }

        public List<FactionMember> GetMembers(Faction faction)
        {
            return GetLoadedMemberships().Where(member => member.GetFactionId() == faction.GetId()).ToList();
        }

        public List<Faction> GetManageableFactions(Character character)
        {
            List<Faction> factions = new List<Faction>();
            foreach (FactionMember membership in GetMemberships(character))
            {
                Faction? faction = _factionService.GetFaction(membership.GetFactionId());
                if (faction == null)
                {
                    continue;
                }

                AddIfMissing(factions, faction);
                if (IsTopRankMember(membership))
                {
                    AddSubFactionsTo(factions, faction);
                }
            }
            return factions;
        }

        public bool IsLeader(Character character, Faction faction)
        {
            return GetMemberships(character).Any(membership => IsTopRankMember(membership) && IsMembershipOfFactionOrAbove(membership, faction));
        }

        public bool HasFactionPermission(Character character, Faction faction, string permission)
        {
            if (IsLeader(character, faction))
            {
                return _factionPermissionService.FactionHasPermission(faction, permission);
            }

            FactionRank? rank = GetMemberRank(GetMembership(character, faction));
            return rank != null && _factionPermissionService.RankHasPermission(rank, permission);
        }

        public string? SetRankPermission(Character actor, Faction faction, FactionRank rank, string permission, bool granted)
        {
            if (!HasFactionPermission(actor, faction, FactionPermissions.ManageRanks))
                return "You are not allowed to manage ranks in this faction.";
            if (!CanManageRank(actor, faction, rank))
                return "You can only manage ranks below your own.";
            if (rank.IsTopRank())
                return "The top rank always has every permission of the faction.";
            if (!HasFactionPermission(actor, faction, permission))
                return "You can only give or take permissions you have yourself.";

            _factionPermissionService.SetRankGrant(rank, permission, granted);
            return null;
        }

        public string? SetSubFactionPermission(Character actor, Faction faction, string permission, bool granted)
        {
            Faction? parent = _factionService.GetParentFaction(faction);
            if (parent == null)
                return "Only a parent faction can grant permissions to this faction.";
            if (!IsLeader(actor, parent))
                return $"Only leaders of {parent.GetName()} can grant permissions to {faction.GetName()}.";
            if (granted && !_factionPermissionService.FactionHasPermission(parent, permission))
                return $"{parent.GetName()} doesn't have this permission itself.";

            _factionPermissionService.SetFactionGrant(faction, permission, granted);
            return null;
        }

        public FactionMember? GetActiveMembership(Character character)
        {
            List<FactionMember> memberships = GetMemberships(character);
            return memberships.Count == 1 ? memberships[0] : memberships.FirstOrDefault(membership => membership.IsActive());
        }

        public string? SetActiveFaction(Character character, Faction faction)
        {
            FactionMember? membership = GetMembership(character, faction);
            if (membership == null)
                return "You are not a member of this faction.";

            foreach (FactionMember otherMembership in GetMemberships(character))
            {
                otherMembership.SetActive(otherMembership.GetId() == membership.GetId());
            }
            return null;
        }

        public bool CanManageRank(Character character, Faction faction, FactionRank rank)
        {
            if (LeadsFactionAbove(character, faction))
            {
                return true;
            }

            FactionRank? ownRank = GetMemberRank(GetMembership(character, faction));
            return ownRank != null && _factionRankService.IsRankBelow(rank, ownRank);
        }

        public List<(FactionRank Rank, int Depth)> GetManageableRanks(Character character, Faction faction)
        {
            return _factionRankService.GetRankTree(faction)
                .Where(entry => CanManageRank(character, faction, entry.Rank))
                .ToList();
        }

        public Faction CreateFaction(string name)
        {
            Faction faction = _factionManager.CreateAndLoad(new FactionModel { Name = name.Trim() });
            _factionRankService.CreateTopRank(faction, TopRankName);
            return faction;
        }

        public string? SetLeader(Character character, Faction faction)
        {
            FactionRank? topRank = _factionRankService.GetTopRank(faction);
            if (topRank == null)
                return "This faction has no top rank.";

            AddOrUpdateMembership(character.GetId(), faction, topRank);
            return null;
        }

        public string? Invite(Character inviter, Player invitee, Faction faction, FactionRank rank)
        {
            string? error = GetInviteError(inviter, invitee, faction, rank);
            if (error != null)
            {
                return error;
            }

            invitee.DestroyComponents<PlayerFactionInvitation>();
            invitee.AddComponent<PlayerFactionInvitation>(faction.GetId(), rank.GetId(), inviter.GetName());
            return null;
        }

        public string? AcceptInvitation(Player player)
        {
            PlayerFactionInvitation? invitation = player.GetComponent<PlayerFactionInvitation>();
            player.DestroyComponents<PlayerFactionInvitation>();

            Faction? faction = invitation == null ? null : _factionService.GetFaction(invitation.GetFactionId());
            FactionRank? rank = invitation == null ? null : _factionRankService.GetRank(invitation.GetRankId());
            if (invitation == null || invitation.IsExpired() || faction == null || rank == null || !player.IsPlayerPlayingAsCharacter())
                return "This invitation is no longer valid.";

            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            if (GetMembership(character, faction) != null)
                return "You are already a member of this faction.";

            AddOrUpdateMembership(character.GetId(), faction, rank);
            return null;
        }

        public void DeclineInvitation(Player player)
        {
            player.DestroyComponents<PlayerFactionInvitation>();
        }

        public string? SetMemberRank(Character actor, FactionMember member, FactionRank newRank)
        {
            Faction? faction = _factionService.GetFaction(member.GetFactionId());
            FactionRank? currentRank = GetMemberRank(member);
            if (faction == null || currentRank == null)
                return "This member no longer exists.";
            if (!HasFactionPermission(actor, faction, FactionPermissions.SetRank))
                return "You are not allowed to change ranks in this faction.";
            if (!CanManageRank(actor, faction, currentRank) || !CanManageRank(actor, faction, newRank))
                return "You can only change ranks below your own.";
            if (IsLastLeader(member) && !newRank.IsTopRank())
                return LastLeaderError;

            member.SetRankId(newRank.GetId());
            _factionPermissionService.NotifyPermissionsChanged(faction);
            return null;
        }

        public string? RemoveMember(Character actor, FactionMember member)
        {
            Faction? faction = _factionService.GetFaction(member.GetFactionId());
            FactionRank? rank = GetMemberRank(member);
            if (faction == null || rank == null)
                return "This member no longer exists.";
            if (!HasFactionPermission(actor, faction, FactionPermissions.Remove))
                return "You are not allowed to remove members from this faction.";
            if (!CanManageRank(actor, faction, rank))
                return "You can only remove members below your own rank.";
            if (IsLastLeader(member))
                return LastLeaderError;

            member.ProcessDeletion();
            _factionPermissionService.NotifyPermissionsChanged(faction, member.GetCharacterId());
            return null;
        }

        public string? Leave(Character character, Faction faction)
        {
            FactionMember? membership = GetMembership(character, faction);
            if (membership == null)
                return "You are not a member of this faction.";
            if (IsLastLeader(membership))
                return LastLeaderError;

            membership.ProcessDeletion();
            _factionPermissionService.NotifyPermissionsChanged(faction, character.GetId());
            return null;
        }

        private string? GetInviteError(Character inviter, Player invitee, Faction faction, FactionRank rank)
        {
            if (!HasFactionPermission(inviter, faction, FactionPermissions.Invite))
                return "You are not allowed to invite players to this faction.";
            if (!CanManageRank(inviter, faction, rank))
                return "You can only invite players to ranks below your own.";
            if (!invitee.IsPlayerPlayingAsCharacter())
                return "That player is not playing as a character.";
            if (GetMembership(invitee.GetPlayerCurrentlyPlayingAsCharacter(), faction) != null)
                return "That player is already a member of this faction.";

            PlayerFactionInvitation? pendingInvitation = invitee.GetComponent<PlayerFactionInvitation>();
            if (pendingInvitation != null && !pendingInvitation.IsExpired())
                return "That player already has an invitation waiting.";
            return null;
        }

        /// <summary>
        /// A membership removed earlier but not yet deleted from the database still holds its row, so it's reused
        /// instead of inserting a second row for the same character and faction.
        /// </summary>
        private void AddOrUpdateMembership(ulong characterId, Faction faction, FactionRank rank)
        {
            FactionMember? membership = _entityManager.GetComponents<FactionMember>()
                .FirstOrDefault(member => member.GetCharacterId() == characterId && member.GetFactionId() == faction.GetId());

            if (membership == null)
            {
                _factionMemberManager.CreateAndLoad(new FactionMemberModel { CharacterId = characterId, FactionId = faction.GetId(), FactionRankId = rank.GetId() });
            }
            else
            {
                membership.ProcessDeletion(false);
                membership.SetActive(false);
                membership.SetRankId(rank.GetId());
            }

            _factionPermissionService.NotifyPermissionsChanged(faction, characterId);
        }

        private List<FactionMember> GetLoadedMemberships()
        {
            return _entityManager.GetComponents<FactionMember>().Where(member => !member.IsDeleted()).ToList();
        }

        private FactionRank? GetMemberRank(FactionMember? member)
        {
            return member == null ? null : _factionRankService.GetRank(member.GetRankId());
        }

        private bool IsTopRankMember(FactionMember membership)
        {
            return GetMemberRank(membership)?.IsTopRank() == true;
        }

        private bool IsLastLeader(FactionMember member)
        {
            return IsTopRankMember(member)
                && GetLoadedMemberships().Count(otherMember => otherMember.GetFactionId() == member.GetFactionId() && IsTopRankMember(otherMember)) == 1;
        }

        /// <summary>
        /// True when the character leads a faction above this one, which makes them leader of everything in it.
        /// </summary>
        private bool LeadsFactionAbove(Character character, Faction faction)
        {
            return GetMemberships(character).Any(membership => IsTopRankMember(membership)
                && membership.GetFactionId() != faction.GetId()
                && IsMembershipOfFactionOrAbove(membership, faction));
        }

        private bool IsMembershipOfFactionOrAbove(FactionMember membership, Faction faction)
        {
            Faction? membershipFaction = _factionService.GetFaction(membership.GetFactionId());
            return membershipFaction != null && _factionService.IsControlledBy(faction, membershipFaction);
        }

        private void AddSubFactionsTo(List<Faction> factions, Faction faction)
        {
            foreach (Faction subFaction in _factionService.GetChildFactions(faction))
            {
                if (factions.Any(existing => existing.GetId() == subFaction.GetId()))
                {
                    continue;
                }

                factions.Add(subFaction);
                AddSubFactionsTo(factions, subFaction);
            }
        }

        private static void AddIfMissing(List<Faction> factions, Faction faction)
        {
            if (!factions.Any(existing => existing.GetId() == faction.GetId()))
            {
                factions.Add(faction);
            }
        }
    }
}
