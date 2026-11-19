using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Fishing.Services
{
    public interface IFishingService
    {
        /// <summary>
        /// Casts the player's line, using one bait and one rod use. Tells the player why when they can't fish here.
        /// </summary>
        void BeginFishing(Player player);

        /// <summary>
        /// Checks every player with a line in the water: cancels casts of players who moved away, starts bites and ends missed ones.
        /// </summary>
        void UpdateFishers();

        /// <summary>
        /// Lands the catch when a fish is biting on the player's line.
        /// </summary>
        void LandBite(Player player);

        /// <summary>
        /// Cancels the player's cast because they got hurt.
        /// </summary>
        void EndFishingAfterDamage(Player player);

        /// <summary>
        /// Cancels the player's cast because they died.
        /// </summary>
        void EndFishingAfterDeath(Player player);

        /// <summary>
        /// Ends the player's fishing without a message, for a player who is leaving the server.
        /// </summary>
        void EndFishing(Player player);
    }
}
