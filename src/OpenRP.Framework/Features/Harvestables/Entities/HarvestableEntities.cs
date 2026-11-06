using SampSharp.Entities;
using System;
using System.Threading;

namespace OpenRP.Framework.Features.Harvestables.Entities
{
    public static class HarvestableEntities
    {
        /// <summary>
        /// Plants placed by the Framework itself (CreateHarvestable). Plants from the biome generator use its own
        /// entity type.
        /// </summary>
        [EntityType]
        public static readonly Guid HarvestablePlantType = new Guid("3B5D7C2E-8F41-4A6B-9D0E-6C1F2A8B4E73");

        private static int HarvestablePlantId = 0;

        public static EntityId GenerateHarvestablePlantId()
        {
            return new EntityId(HarvestablePlantType, Interlocked.Increment(ref HarvestablePlantId));
        }
    }
}
