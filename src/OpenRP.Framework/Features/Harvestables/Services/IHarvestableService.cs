using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Harvestables.Services
{
    public interface IHarvestableService
    {
        /// <summary>
        /// True when objects of this model are a registered harvestable plant.
        /// </summary>
        bool IsHarvestableModel(int modelId);

        /// <summary>
        /// The names players can use with /harvest.
        /// </summary>
        IEnumerable<string> GetResourceNames();

        /// <summary>
        /// Turns an entity whose object the caller already created (for example the biome generator) into a
        /// harvestable plant, with its text label. False when the model isn't harvestable.
        /// </summary>
        bool AddHarvestable(EntityId entityId, int modelId, Vector3 position);

        /// <summary>
        /// Places a new harvestable plant: its entity, object and text label. False when the model isn't harvestable.
        /// </summary>
        bool CreateHarvestable(int modelId, Vector3 position, Vector3 rotation);

        /// <summary>
        /// Starts harvesting the nearest plant of this resource, if the player can.
        /// </summary>
        void BeginHarvest(Player player, string resourceName);

        /// <summary>
        /// Completes every harvest whose time is up.
        /// </summary>
        void EndFinishedHarvests();
    }
}
