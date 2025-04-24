using OpenRP.Streamer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Fishing.Services
{
    public interface IFishHotspotService
    {
        Task<DynamicObject> CreateRandomHotspot();
    }
}
