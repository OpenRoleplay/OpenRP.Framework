using OpenRP.Framework.Features.Harvestables.Components;
using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.Harvestables.Services
{
    public class IndianHempHarvestableModel : HarvestableModel<IndianHempPlant>
    {
        private const int IndianHempObjectModelId = 19473;

        public IndianHempHarvestableModel(IEntityManager entityManager)
            : base(entityManager)
        {
        }

        public override string ResourceName => "hemp";
        public override int[] ResourceObjectModelIds => new[] { IndianHempObjectModelId };
        public override TimeSpan HarvestDuration => TimeSpan.FromSeconds(10);
    }
}
