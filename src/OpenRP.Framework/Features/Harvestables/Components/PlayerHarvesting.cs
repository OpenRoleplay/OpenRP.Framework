using OpenRP.Framework.Features.Harvestables.Entities;
using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.Harvestables.Components
{
    /// <summary>
    /// Marks a player who is harvesting a plant, until the harvest is finished.
    /// </summary>
    public class PlayerHarvesting : Component
    {
        private readonly HarvestablePlant _plant;
        private readonly IHarvestableModel _harvestableModel;
        private readonly DateTime _finishesAt;

        public PlayerHarvesting(HarvestablePlant plant, IHarvestableModel harvestableModel)
        {
            _plant = plant;
            _harvestableModel = harvestableModel;
            _finishesAt = DateTime.UtcNow.Add(harvestableModel.HarvestDuration);
        }

        public HarvestablePlant GetPlant()
        {
            return _plant;
        }

        public IHarvestableModel GetHarvestableModel()
        {
            return _harvestableModel;
        }

        public bool IsFinished()
        {
            return DateTime.UtcNow >= _finishesAt;
        }
    }
}
