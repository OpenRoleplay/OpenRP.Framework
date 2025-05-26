using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Harvestables.Entities
{
    public interface IHarvestable
    {
        /// <summary>
        /// The name of the resource (e.g. "hemp"). Used for the /harvest command.
        /// </summary>
        string ResourceName { get; }

        /// <summary>
        /// The object model id of the resource.
        /// </summary>
        int ResourceObjectModelId { get; }

        /// <summary>
        /// Create the harvestable in the world.
        /// </summary>
        /// <param name="positions">The positions of the harvestable.</param>
        /// <param name="rotations">The rotations of the harvestable.</param>
        void CreateHarvestable(Vector3 positions, Vector3 rotations);

        /// <summary>
        /// Performs code at the begin of a player harvesting the resource
        /// </summary>
        /// <param name="player">The player harvesting.</param>
        void BeginHarvest(Player player);

        /// <summary>
        /// Performs code at the end of a player harvesting the resource
        /// </summary>
        /// <param name="player">The player harvesting.</param>
        void EndHarvest(Player player);
    }
}
