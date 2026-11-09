using OpenRP.Framework.Features.Actors.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Actors.Systems
{
    public class ActorSystem : ISystem
    {
        // Refreshes wait about 1.5 seconds, so checking twice a second is precise enough.
        private const int SpawnPoseRefreshCheckIntervalInMilliseconds = 500;

        [Event]
        public void OnGameModeInit(IActorService actorService)
        {
            actorService.LoadActors();
        }

        [Event]
        public void OnActorStreamIn(Actor actor, Player forPlayer, IActorService actorService)
        {
            actorService.ScheduleSpawnPoseRefresh(actor);
        }

        [Timer(SpawnPoseRefreshCheckIntervalInMilliseconds)]
        public void RefreshDueSpawnPoses(IActorService actorService)
        {
            actorService.RefreshDueSpawnPoses();
        }
    }
}
