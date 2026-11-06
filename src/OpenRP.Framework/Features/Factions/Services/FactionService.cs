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

        public FactionService(IEntityManager entityManager)
        {
            _entityManager = entityManager;
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
    }
}
