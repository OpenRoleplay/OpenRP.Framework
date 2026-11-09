using Microsoft.Extensions.Logging;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Factions.Entities;
using SampSharp.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Factions.Services
{
    public class FactionService : IFactionService
    {
        private readonly IEntityManager _entityManager;
        private readonly ILogger<FactionService> _logger;

        public FactionService(IEntityManager entityManager, ILogger<FactionService> logger)
        {
            _entityManager = entityManager;
            _logger = logger;
        }

        public Faction? GetFaction(ulong factionId)
        {
            return _entityManager.GetComponent<Faction>(FactionEntities.GetFactionId((int)factionId));
        }

        public Faction? GetParentFaction(Faction faction)
        {
            ulong? parentFactionId = faction.GetParentFactionId();
            return parentFactionId.HasValue ? GetFaction(parentFactionId.Value) : null;
        }

        public List<Faction> GetChildFactions(Faction faction)
        {
            return _entityManager.GetComponents<Faction>()
                .Where(childFaction => childFaction.GetParentFactionId() == faction.GetId())
                .ToList();
        }

        public Faction GetTopFaction(Faction faction)
        {
            return GetFactionAndAncestors(faction).Last();
        }

        public bool IsControlledBy(Faction faction, Faction controller)
        {
            return GetFactionAndAncestors(faction).Any(ancestor => ancestor.GetId() == controller.GetId());
        }

        /// <summary>
        /// The faction followed by every loaded faction above it, from its parent up to the top. Stops at a parent that
        /// isn't loaded, and at a loop in the data (A above B above A), which would otherwise never end.
        /// </summary>
        private List<Faction> GetFactionAndAncestors(Faction faction)
        {
            List<Faction> factionAndAncestors = new List<Faction> { faction };
            HashSet<ulong> visitedIds = new HashSet<ulong> { faction.GetId() };

            Faction? parent = GetParentFaction(faction);
            while (parent != null)
            {
                if (!visitedIds.Add(parent.GetId()))
                {
                    _logger.LogWarning("Faction {FactionId} is part of a loop of parent factions; check ParentFactionId in the database.", parent.GetId());
                    break;
                }
                factionAndAncestors.Add(parent);
                parent = GetParentFaction(parent);
            }
            return factionAndAncestors;
        }
    }
}
