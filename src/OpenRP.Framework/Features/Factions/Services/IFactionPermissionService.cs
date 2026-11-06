using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Permissions.Services;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Factions.Services
{
    /// <summary>
    /// What factions and ranks may do. Only grants are stored; what a faction or rank actually has is worked out from
    /// the top down: every faction has the defaults (the "Faction" permission group), a sub-faction's grants only count
    /// while its parent has them, and a rank's grants only count while its faction has them. The top rank has everything
    /// its faction has. This service doesn't check who is asking; FactionMembershipService does that.
    /// </summary>
    public interface IFactionPermissionService
    {
        /// <summary>
        /// Creates the faction.* permissions when missing, puts them in the "Faction" group and reads all permissions.
        /// Called once at startup.
        /// </summary>
        void Initialize(IPermissionService permissionService);

        bool IsKnownPermission(string permission);

        bool IsDefaultPermission(string permission);

        /// <summary>
        /// The description stored with the permission, or its name when it has none.
        /// </summary>
        string GetPermissionDescription(string permission);

        /// <summary>
        /// The names of the permissions the faction has right now, sorted.
        /// </summary>
        List<string> GetFactionPermissions(Faction faction);

        bool FactionHasPermission(Faction faction, string permission);

        /// <summary>
        /// The names of the permissions the rank has right now, sorted.
        /// </summary>
        List<string> GetRankPermissions(FactionRank rank);

        bool RankHasPermission(FactionRank rank, string permission);

        /// <summary>
        /// True when the faction has a grant for this permission, whether it counts right now or not.
        /// </summary>
        bool IsGrantedToFaction(Faction faction, string permission);

        bool IsGrantedToRank(FactionRank rank, string permission);

        /// <summary>
        /// Grants that don't count right now because the parent faction doesn't have them.
        /// </summary>
        List<string> GetInactiveFactionGrants(Faction faction);

        /// <summary>
        /// Grants that don't count right now because the rank's faction doesn't have them.
        /// </summary>
        List<string> GetInactiveRankGrants(FactionRank rank);

        void SetFactionGrant(Faction faction, string permission, bool granted);

        void SetRankGrant(FactionRank rank, string permission, bool granted);

        /// <summary>
        /// Publishes OnFactionPermissionsChanged for the members of the faction and its sub-factions, plus these characters
        /// (for example someone who just left), so their command permissions are reloaded.
        /// </summary>
        void NotifyPermissionsChanged(Faction faction, params ulong[] alsoCharacterIds);
    }
}
