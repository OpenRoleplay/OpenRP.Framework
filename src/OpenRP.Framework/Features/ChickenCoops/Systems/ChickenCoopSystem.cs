using OpenRP.Framework.Features.ChickenCoops.Services;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.ChickenCoops.Systems
{
    public class ChickenCoopSystem : ISystem
    {
        // Collection steps and breeding have their own due times; these only decide how often they're checked.
        private const int EggCollectionCheckIntervalInMilliseconds = 500;
        private const int CoopUpdateIntervalInMilliseconds = 60000;

        [Timer(EggCollectionCheckIntervalInMilliseconds)]
        public void ContinueEggCollections(IChickenCoopService chickenCoopService)
        {
            chickenCoopService.ContinueEggCollections();
        }

        [Timer(CoopUpdateIntervalInMilliseconds)]
        public void UpdateChickenCoops(IChickenCoopService chickenCoopService)
        {
            chickenCoopService.UpdateChickenCoops();
        }
    }
}
