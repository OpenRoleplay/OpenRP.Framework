using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.Factions.Entities
{
    public static class FactionEntities
    {
        [EntityType]
        public static readonly Guid FactionType = new Guid("ACDFB33C-E5B5-40F6-90FE-A83DA1CBE7FD");

        public static EntityId GetFactionId(int factionId)
        {
            return new EntityId(FactionType, factionId);
        }
    }
}
