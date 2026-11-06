using OpenRP.Framework.Features.Harvestables.Services;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Harvestables.Systems
{
    public class HarvestableSystem : ISystem
    {
        private const int HarvestCheckIntervalInMilliseconds = 1000;

        [Timer(HarvestCheckIntervalInMilliseconds)]
        public void EndFinishedHarvests(IHarvestableService harvestableService)
        {
            harvestableService.EndFinishedHarvests();
        }
    }
}
