using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.VirtualWorlds.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using OpenRP.OpenMp.Streamer;

namespace OpenRP.Framework.Features.Actors.Services
{
    public class ActorService : IActorService
    {
        // The streamer creates objects on its own tick (stream_rate: once a second), so the mapping under an actor can
        // reach a player up to about a second after the actor; this leaves a margin.
        private static readonly TimeSpan SpawnPoseRefreshDelay = TimeSpan.FromMilliseconds(1500);
        private const int OutsideVirtualWorld = 0;
        private const float NameTagHeight = 1.4f;
        private const float NameTagDrawDistance = 30.0f;

        private readonly IWorldService _worldService;
        private readonly IStreamerService _streamerService;
        private readonly IVirtualWorldManager _virtualWorldManager;
        private readonly IEntityManager _entityManager;

        public ActorService(IWorldService worldService, IStreamerService streamerService, IVirtualWorldManager virtualWorldManager, IEntityManager entityManager)
        {
            _worldService = worldService;
            _streamerService = streamerService;
            _virtualWorldManager = virtualWorldManager;
            _entityManager = entityManager;
        }

        public List<ServerActor> GetServerActors()
        {
            return _entityManager.GetComponents<ServerActor>().Where(serverActor => !serverActor.IsDeleted()).ToList();
        }

        public int GetActorVirtualWorld(ServerActor serverActor)
        {
            ulong? mainMenuSceneId = serverActor.GetMainMenuSceneId();
            if (mainMenuSceneId.HasValue)
            {
                return _virtualWorldManager.GetMainMenuSceneVirtualWorld(mainMenuSceneId.Value);
            }

            ulong? insidePropertyId = serverActor.GetInsidePropertyId();
            return insidePropertyId.HasValue ? _virtualWorldManager.GetPropertyVirtualWorld(insidePropertyId.Value) : OutsideVirtualWorld;
        }

        public void CreateActorInWorld(ServerActor serverActor)
        {
            DestroyActorInWorld(serverActor);

            int virtualWorld = GetActorVirtualWorld(serverActor);
            Actor actor = _worldService.CreateActor(serverActor.GetSkin(), serverActor.GetPosition(), serverActor.GetAngle(), serverActor.Entity);
            actor.VirtualWorld = virtualWorld;
            serverActor.ApplyIdleAnimation();
            CreateNameTag(serverActor, virtualWorld);
        }

        public void DestroyActorInWorld(ServerActor serverActor)
        {
            foreach (Actor actor in serverActor.GetComponentsInChildren<Actor>().ToList())
            {
                actor.DestroyEntity();
            }
            foreach (DynamicTextLabel nameTag in serverActor.GetComponentsInChildren<DynamicTextLabel>().ToList())
            {
                nameTag.DestroyEntity();
            }
        }

        private void CreateNameTag(ServerActor serverActor, int virtualWorld)
        {
            if (String.IsNullOrWhiteSpace(serverActor.GetName()))
            {
                return;
            }

            _streamerService.CreateDynamicTextLabel(serverActor.GetName(), Color.WhiteSmoke, serverActor.GetPosition() + new Vector3(0, 0, NameTagHeight),
                NameTagDrawDistance, testLos: true, virtualWorld: virtualWorld, interior: serverActor.GetInterior(), parent: serverActor.Entity);
        }

        public void ScheduleSpawnPoseRefresh(Actor actor)
        {
            ServerActor? serverActor = actor.GetComponentInParent<ServerActor>();
            if (serverActor == null)
            {
                return;
            }

            ActorSpawnPoseRefresh? refresh = serverActor.GetComponent<ActorSpawnPoseRefresh>();
            if (refresh != null)
            {
                refresh.Postpone(SpawnPoseRefreshDelay);
                return;
            }

            serverActor.AddComponent<ActorSpawnPoseRefresh>(SpawnPoseRefreshDelay);
        }

        public void RefreshDueSpawnPoses()
        {
            foreach (ActorSpawnPoseRefresh refresh in _entityManager.GetComponents<ActorSpawnPoseRefresh>().ToList())
            {
                if (!refresh.IsDue())
                {
                    continue;
                }

                refresh.GetComponent<ServerActor>()?.ResetToSpawnPose();
                refresh.Destroy();
            }
        }
    }
}
