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
        public static EntityId GenerateFishHotspotId()
        {
            return EntityId.NewEntityId();
        }
    }
}
