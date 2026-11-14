using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.FlyMode.Services
{
    public interface IFlyModeMovementService
    {
        /// <summary>
        /// Moves a flying player's camera object from their keys. Does nothing for players who aren't flying.
        /// </summary>
        void UpdateFlight(Player player);
    }
}
