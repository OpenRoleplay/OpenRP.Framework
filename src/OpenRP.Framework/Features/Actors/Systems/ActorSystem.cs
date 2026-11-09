using OpenRP.Framework.Features.Actors.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Actors.Systems
{
    public class ActorSystem : ISystem
    {
        // Refreshes wait about 1.5 seconds, so checking twice a second is precise enough.
        private const int SpawnPoseRefreshCheckIntervalInMilliseconds = 500;
        private const int SyncIntervalInMilliseconds = 60000;

        [Event]
        public void OnGameModeInit(IActorManager actorManager)
        {
            actorManager.LoadAll();
        }

        [Timer(SyncIntervalInMilliseconds)]
        public void ProcessChanges(IActorManager actorManager)
        {
            actorManager.ProcessChanges();
        }

        [Event]
        public void OnGameModeExit(IActorManager actorManager)
        {
            actorManager.SaveNow();
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
