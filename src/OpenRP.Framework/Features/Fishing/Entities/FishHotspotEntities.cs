using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Fishing.Entities
{
    public static class FishHotspotEntities
    {
        private static int FishHotspotId = 0;

        [EntityType]
        public static readonly Guid FishHotspotType = new Guid("9C4B75B8-C492-4E26-9E29-0B1A83D1D1A7");

        public static EntityId GenerateFishHotspotId()
        {
            return new EntityId(FishHotspotType, FishHotspotId++);
        }
    }
}
