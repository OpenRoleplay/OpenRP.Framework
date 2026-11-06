using OpenRP.Framework.Features.Harvestables.Components;
using OpenRP.Framework.Features.Harvestables.Entities;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Harvestables.Services
{
    /// <summary>
    /// Base for plant types: a new plant only declares its data. TPlant is its component, which takes the resource
    /// name and the label position in its constructor.
    /// </summary>
    public abstract class HarvestableModel<TPlant> : IHarvestableModel
        where TPlant : HarvestablePlant
    {
        private readonly IEntityManager _entityManager;

        protected HarvestableModel(IEntityManager entityManager)
        {
            _entityManager = entityManager;
        }

        public abstract string ResourceName { get; }
        public abstract int[] ResourceObjectModelIds { get; }
        public abstract TimeSpan HarvestDuration { get; }
        public virtual string HarvestAnimationLibrary => "BOMBER";
        public virtual string HarvestAnimationName => "BOM_Plant_Loop";

        public HarvestablePlant AddPlant(EntityId entityId, Vector3 labelPosition)
        {
            return _entityManager.AddComponent<TPlant>(entityId, ResourceName, labelPosition);
        }

        public IEnumerable<HarvestablePlant> GetPlants()
        {
            return _entityManager.GetComponents<TPlant>();
        }
    }
}
