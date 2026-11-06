using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Factions.Entities;
using OpenRP.Framework.Features.Permissions.Services;
using SampSharp.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Factions.Services
{
    public class FactionRankService : IFactionRankService
    {
        private const int RankNameMaxLength = 64;

        private readonly IEntityManager _entityManager;
        private readonly IFactionRankManager _factionRankManager;
        private readonly ILogger<FactionRankService> _logger;

        public FactionRankService(IEntityManager entityManager, IFactionRankManager factionRankManager, ILogger<FactionRankService> logger)
        {
            _entityManager = entityManager;
            _factionRankManager = factionRankManager;
            _logger = logger;
        }

        public FactionRank? GetRank(ulong rankId)
        {
            FactionRank? rank = _entityManager.GetComponent<FactionRank>(FactionEntities.GetFactionRankId((int)rankId));
            return rank == null || rank.IsDeleted() ? null : rank;
        }

        public FactionRank? GetTopRank(Faction faction)
        {
            return GetRanks(faction.GetId()).FirstOrDefault(rank => rank.IsTopRank());
        }

        public List<(FactionRank Rank, int Depth)> GetRankTree(Faction faction)
        {
            FactionRank? topRank = GetTopRank(faction);
            if (topRank == null)
            {
                return new List<(FactionRank Rank, int Depth)>();
            }

            List<(FactionRank Rank, int Depth)> tree = new List<(FactionRank Rank, int Depth)> { (topRank, 0) };
            tree.AddRange(GetRanksBelow(topRank).Select(entry => (entry.Rank, entry.Depth + 1)));
            return tree;
        }

        public List<(FactionRank Rank, int Depth)> GetRanksBelow(FactionRank rank)
        {
            List<(FactionRank Rank, int Depth)> ranksBelow = new List<(FactionRank Rank, int Depth)>();
            AddSubRanksTo(ranksBelow, rank, 0, new HashSet<ulong> { rank.GetId() });
            return ranksBelow;
        }

        public bool IsRankBelow(FactionRank rank, FactionRank otherRank)
        {
            return GetRanksBelow(otherRank).Any(entry => entry.Rank.GetId() == rank.GetId());
        }

        public FactionRank CreateTopRank(Faction faction, string name)
        {
            return _factionRankManager.CreateAndLoad(new FactionRankModel { FactionId = faction.GetId(), Name = name });
        }

        public string? CreateRank(FactionRank parentRank, string name)
        {
            string? error = GetRankNameError(name);
            if (error != null)
            {
                return error;
            }

            _factionRankManager.CreateAndLoad(new FactionRankModel { FactionId = parentRank.GetFactionId(), ParentRankId = parentRank.GetId(), Name = name.Trim() });
            return null;
        }

        public string? RenameRank(FactionRank rank, string name)
        {
            string? error = GetRankNameError(name);
            if (error != null)
            {
                return error;
            }

            rank.SetName(name.Trim());
            return null;
        }

        public string? MoveRank(FactionRank rank, FactionRank newParentRank)
        {
            if (rank.IsTopRank())
                return "The top rank can't be moved.";
            if (rank.GetId() == newParentRank.GetId() || IsRankBelow(newParentRank, rank))
                return "A rank can't be moved below itself or one of its own sub-ranks.";

            rank.SetParentRankId(newParentRank.GetId());
            return null;
        }

        public string? DeleteRank(FactionRank rank)
        {
            if (rank.IsTopRank())
                return "The top rank can't be deleted.";
            if (GetSubRanks(rank).Any())
                return "Delete or move its sub-ranks first.";
            if (HasMembers(rank))
                return "Give its members another rank first.";

            rank.ProcessDeletion();
            return null;
        }

        private void AddSubRanksTo(List<(FactionRank Rank, int Depth)> ranksBelow, FactionRank rank, int depth, HashSet<ulong> visitedIds)
        {
            foreach (FactionRank subRank in GetSubRanks(rank))
            {
                // A loop in the data (A below B below A) would otherwise never end.
                if (!visitedIds.Add(subRank.GetId()))
                {
                    _logger.LogWarning("Faction rank {RankId} is part of a loop of parent ranks; check ParentRankId in the database.", subRank.GetId());
                    continue;
                }

                ranksBelow.Add((subRank, depth));
                AddSubRanksTo(ranksBelow, subRank, depth + 1, visitedIds);
            }
        }

        private List<FactionRank> GetRanks(ulong factionId)
        {
            return _entityManager.GetComponents<FactionRank>()
                .Where(rank => rank.GetFactionId() == factionId && !rank.IsDeleted())
                .ToList();
        }

        private List<FactionRank> GetSubRanks(FactionRank rank)
        {
            return GetRanks(rank.GetFactionId())
                .Where(subRank => subRank.GetParentRankId() == rank.GetId())
                .OrderBy(subRank => subRank.GetId())
                .ToList();
        }

        private bool HasMembers(FactionRank rank)
        {
            return _entityManager.GetComponents<FactionMember>().Any(member => member.GetRankId() == rank.GetId() && !member.IsDeleted());
        }

        private static string? GetRankNameError(string name)
        {
            string trimmedName = name.Trim();
            if (trimmedName.Length == 0)
                return "The rank name can't be empty.";
            if (trimmedName.Length > RankNameMaxLength)
                return $"The rank name can be at most {RankNameMaxLength} characters long.";
            return null;
        }
    }
}
