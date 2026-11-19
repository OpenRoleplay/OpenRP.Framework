using OpenRP.Framework.Features.Fishing.Entities;
using OpenRP.Framework.Features.Fishing.Services;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Fishing.Systems
{
    public class TrawlSystem : ISystem
    {
        [Timer(FishingSettings.TrawlUpdateIntervalInMilliseconds)]
        public void UpdateTrawlers(ITrawlService trawlService)
        {
            trawlService.UpdateTrawlers();
        }
    }
}
