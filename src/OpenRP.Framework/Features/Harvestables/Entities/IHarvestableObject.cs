using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Harvestables.Entities
{
    /// <summary>
    /// What the harvest code needs from any plant, whatever its type.
    /// </summary>
    public interface IHarvestableObject
    {
        string GetResourceName();
        bool IsPlayerNearby(Player player);
        bool HasHarvestsLeft();

        /// <summary>
        /// Takes one harvest (for example a leaf) off the plant.
        /// </summary>
        void RemoveOneHarvest();
    }
}
