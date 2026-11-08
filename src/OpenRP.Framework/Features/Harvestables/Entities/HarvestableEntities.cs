using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.Harvestables.Entities
{
    public static class HarvestableEntities
    {
        public static EntityId GenerateHarvestablePlantId()
        {
            return EntityId.NewEntityId();
        }
    }
}
