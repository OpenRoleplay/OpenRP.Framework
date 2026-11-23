using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Fishing.Services
{
    public interface ITrawlService
    {
        /// <summary>
        /// Lowers the net of the Reefer the player drives, or raises it early when it's already down.
        /// </summary>
        void ToggleTrawl(Player player);

        /// <summary>
        /// Counts the sailing time of every trawling driver, and brings up the net when it's done.
        /// </summary>
        void UpdateTrawlers();
    }
}
