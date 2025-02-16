using SampSharp.Entities;
using SampSharp.Streamer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Fishing.Components
{
    public class FishHotspot : Component
    {
        public FishHotspot()
        {
        }

        public DynamicObject GetDynamicObject()
        {
            return this.GetComponentInChildren<DynamicObject>();
        }
    }
}
