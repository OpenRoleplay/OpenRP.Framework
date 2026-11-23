using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.FlyMode.Services
{
    public interface IFlyModeRestoreService
    {
        /// <summary>
        /// Puts a player who has just landed back the way they were before they flew. Does nothing for anyone else.
        /// </summary>
        void RestoreAfterSpawn(Player player);
    }
}
