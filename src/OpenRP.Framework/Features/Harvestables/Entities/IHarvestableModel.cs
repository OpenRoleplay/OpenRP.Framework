using OpenRP.Framework.Features.Harvestables.Components;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Harvestables.Entities
{
    /// <summary>
    /// Describes one type of harvestable plant. Derive from HarvestableModel&lt;TPlant&gt; and register it with
    /// AddHarvestableModel&lt;T&gt;() to add a new plant.
    /// </summary>
    public interface IHarvestableModel
    {
        /// <summary>
        /// The name players use with /harvest, for example "hemp". One word, so the command parses cleanly.
        /// </summary>
        string ResourceName { get; }

        /// <summary>
        /// The object models that are this plant.
        /// </summary>
        int[] ResourceObjectModelIds { get; }

        TimeSpan HarvestDuration { get; }
        string HarvestAnimationLibrary { get; }
        string HarvestAnimationName { get; }

        /// <summary>
        /// Adds this plant's component to the entity and returns it.
        /// </summary>
        HarvestablePlant AddPlant(EntityId entityId, Vector3 labelPosition);

        /// <summary>
        /// All plants of this type in the world.
        /// </summary>
        IEnumerable<HarvestablePlant> GetPlants();
    }
}
