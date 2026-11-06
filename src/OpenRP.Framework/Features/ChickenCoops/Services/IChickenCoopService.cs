using OpenRP.Framework.Features.ChickenCoops.Components;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.ChickenCoops.Services
{
    public interface IChickenCoopService
    {
        /// <summary>
        /// Places a chicken coop with its text label at this position, for example on a coop object of a mapping.
        /// </summary>
        ChickenCoop CreateChickenCoop(Vector3 position);

        /// <summary>
        /// Starts collecting an egg from the coop the player is at, or tells them why they can't.
        /// </summary>
        void BeginCollectEgg(Player player);

        /// <summary>
        /// Moves every egg collection whose next step is due one step further. Called on a timer.
        /// </summary>
        void ContinueEggCollections();

        /// <summary>
        /// Lets chickens breed and lay eggs when it's time to. Called on a timer.
        /// </summary>
        void UpdateChickenCoops();
    }
}
