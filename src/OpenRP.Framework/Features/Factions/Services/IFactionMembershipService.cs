using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Factions.Components;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Factions.Services
{
    /// <summary>
    /// Who is in which faction, and who may manage what. You can only manage ranks and members below your own rank;
    /// leaders of a faction also lead all of its sub-factions. Actions return an error message, or null when they worked.
    /// </summary>
    public interface IFactionMembershipService
    {
        List<FactionMember> GetMemberships(Character character);

        FactionMember? GetMembership(Character character, Faction faction);

        List<FactionMember> GetMembers(Faction faction);

        /// <summary>
        /// The factions the character can open in /faction: their own, and the sub-factions of factions they lead.
        /// </summary>
        List<Faction> GetManageableFactions(Character character);

        /// <summary>
        /// True for members with the faction's top rank, and for leaders of a faction above it.
        /// </summary>
        bool IsLeader(Character character, Faction faction);

        /// <summary>
        /// True when the character has this permission in the faction: through their rank, or as a leader through
        /// everything the faction has.
        /// </summary>
        bool HasFactionPermission(Character character, Faction faction, string permission);

        /// <summary>
        /// Gives a rank a permission or takes it away. Needs the manage-ranks permission, a rank below your own, and the
        /// permission itself.
        /// </summary>
        string? SetRankPermission(Character actor, Faction faction, FactionRank rank, string permission, bool granted);

        /// <summary>
        /// Grants a sub-faction a permission or takes it away. Only for leaders of its parent faction, and only
        /// permissions the parent has.
        /// </summary>
        string? SetSubFactionPermission(Character actor, Faction faction, string permission, bool granted);

        /// <summary>
        /// The faction that faction chat goes to: the only membership, or the one chosen as active. Null when the
        /// character is in several factions without choosing.
        /// </summary>
        FactionMember? GetActiveMembership(Character character);

        string? SetActiveFaction(Character character, Faction faction);

        /// <summary>
        /// True when the rank is below the character's own rank in this faction, or the character leads a faction above it.
        /// </summary>
        bool CanManageRank(Character character, Faction faction, FactionRank rank);

        /// <summary>
        /// The ranks the character may give to others or manage, as a tree in display order.
        /// </summary>
        List<(FactionRank Rank, int Depth)> GetManageableRanks(Character character, Faction faction);

        /// <summary>
        /// Creates a faction with its top rank. Returns the new faction.
        /// </summary>
        Faction CreateFaction(string name);

        /// <summary>
        /// Makes the character a member with the faction's top rank, for admins setting up a faction.
        /// </summary>
        string? SetLeader(Character character, Faction faction);

        string? Invite(Character inviter, Player invitee, Faction faction, FactionRank rank);

        string? AcceptInvitation(Player player);

        void DeclineInvitation(Player player);

        string? SetMemberRank(Character actor, FactionMember member, FactionRank newRank);

        string? RemoveMember(Character actor, FactionMember member);

        string? Leave(Character character, Faction faction);
    }
}
