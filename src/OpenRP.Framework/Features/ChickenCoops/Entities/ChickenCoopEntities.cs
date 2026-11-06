using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.ChickenCoops.Entities
{
    public static class ChickenCoopEntities
    {
        [EntityType]
        public static readonly Guid ChickenCoopType = new Guid("5E2C8A61-3F4B-4D7E-9A1C-B86F2D04E7A3");

        private static int ChickenCoopId = 0;

        public static EntityId GenerateChickenCoopId()
        {
            return new EntityId(ChickenCoopType, Interlocked.Increment(ref ChickenCoopId));
        }
    }
}
