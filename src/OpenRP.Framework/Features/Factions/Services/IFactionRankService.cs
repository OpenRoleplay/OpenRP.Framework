using OpenRP.Framework.Features.Factions.Components;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Factions.Services
{
    /// <summary>
    /// The rank tree of each faction. Rank permissions are in IFactionPermissionService.
    /// </summary>
    public interface IFactionRankService
    {
        /// <summary>
        /// The loaded rank with this id, or null.
        /// </summary>
        FactionRank? GetRank(ulong rankId);

        /// <summary>
        /// The faction's top (leader) rank, or null when it has none yet.
        /// </summary>
        FactionRank? GetTopRank(Faction faction);

        /// <summary>
        /// All ranks of the faction as a tree in display order: each rank followed by its sub-ranks, with its depth.
        /// </summary>
        List<(FactionRank Rank, int Depth)> GetRankTree(Faction faction);

        /// <summary>
        /// The ranks below this one, as a tree in display order. The depth is counted from this rank.
        /// </summary>
        List<(FactionRank Rank, int Depth)> GetRanksBelow(FactionRank rank);

        /// <summary>
        /// True when the rank is somewhere below the other rank, not the other rank itself.
        /// </summary>
        bool IsRankBelow(FactionRank rank, FactionRank otherRank);

        /// <summary>
        /// Creates the faction's top rank. Only for a new faction.
        /// </summary>
        FactionRank CreateTopRank(Faction faction, string name);

        /// <summary>
        /// Creates a rank directly below the parent rank. Returns an error message, or null when it was created.
        /// </summary>
        string? CreateRank(FactionRank parentRank, string name);

        string? RenameRank(FactionRank rank, string name);

        /// <summary>
        /// Moves the rank, with its sub-ranks, directly below the new parent rank.
        /// </summary>
        string? MoveRank(FactionRank rank, FactionRank newParentRank);

        /// <summary>
        /// Deletes a rank without members or sub-ranks.
        /// </summary>
        string? DeleteRank(FactionRank rank);
    }
}
