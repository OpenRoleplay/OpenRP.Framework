using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.ChickenCoops.Entities
{
    public static class ChickenCoopEntities
    {
        public static EntityId GenerateChickenCoopId()
        {
            return EntityId.NewEntityId();
        }
    }
}
