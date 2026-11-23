using OpenRP.Framework.Features.FlyMode.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using SampSharp.OpenMp.Core.Std.Chrono;

namespace OpenRP.Framework.Features.FlyMode.Systems
{
    public class FlyModeSystem : ISystem
    {
        /// <summary>
        /// The update event passes the time point too; a handler without that parameter is never called.
        /// </summary>
        [Event]
        public void OnPlayerUpdate(Player player, TimePoint now, IFlyModeMovementService flyModeMovementService)
        {
            flyModeMovementService.UpdateFlight(player);
        }

        [Event]
        public void OnPlayerSpawn(Player player, IFlyModeRestoreService flyModeRestoreService)
        {
            flyModeRestoreService.RestoreAfterSpawn(player);
        }
    }
}
