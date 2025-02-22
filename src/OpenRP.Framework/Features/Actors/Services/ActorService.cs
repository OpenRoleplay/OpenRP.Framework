using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Actors.Entities;
using OpenRP.Framework.Features.VirtualWorlds.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using SampSharp.Streamer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace OpenRP.Framework.Features.Actors.Services
{
    public class ActorService : IActorService
    {
        private BaseDataContext _dataContext;
        private IWorldService _worldService;
        private IStreamerService _streamerService;
        private IVirtualWorldManager _virtualWorldService;
        private IEntityManager _entityManager;

        public ActorService(BaseDataContext dataContext, IWorldService worldService, IStreamerService streamerService, IVirtualWorldManager virtualWorldService, IEntityManager entityManager)
        {
            _dataContext = dataContext;
            _worldService = worldService;
            _streamerService = streamerService;
            _virtualWorldService = virtualWorldService;
            _entityManager = entityManager;
        }

        public List<ActorModel> GetActors()
        {
            return _dataContext.Actors
                .Cacheable(CacheExpirationMode.Sliding, TimeSpan.FromHours(12), $"Actors")
                .Include(i => i.ActorCharacter)
                .Include(i => i.ActorPrompt)
                .Include(i => i.ActorRelationships)
                .Include(i => i.ActorLinkedToMainMenuScene)
                .ToList();
        }

        public void LoadActors()
        {
            List<ActorModel> actorModels = GetActors();

            foreach (ActorModel actorModel in actorModels)
            {
                EntityId serverActorEntityId = ActorEntities.GetServerActorId((int)actorModel.Id);
                _entityManager.Create(serverActorEntityId);

                ServerActor serverActor = _entityManager.AddComponent<ServerActor>(serverActorEntityId, actorModel);

                Actor gameActor = _worldService.CreateActor(actorModel.Skin, new Vector3(actorModel.X, actorModel.Y, actorModel.Z), actorModel.Angle, serverActorEntityId);
                gameActor.ApplyAnimation(actorModel.AnimLibrary, actorModel.AnimName, actorModel.AnimSpeed, true, true, true, true, 0);

                if (actorModel.ActorCharacter != null)
                {
                    int virtualWorld = -1;

                    if (actorModel.ActorLinkedToMainMenuScene != null)
                    {
                        virtualWorld = _virtualWorldService.GetMainMenuSceneVirtualWorld(actorModel.ActorLinkedToMainMenuScene.Id);
                    }

                    _streamerService.CreateDynamicTextLabel(serverActor.GetName(), Color.WhiteSmoke, new Vector3(actorModel.X, actorModel.Y, actorModel.Z + 1.4f), 30.0f, testLos: true, virtualWorld: virtualWorld, parent: serverActorEntityId);
                }

                if (actorModel.ActorLinkedToMainMenuScene != null)
                {
                    gameActor.VirtualWorld = _virtualWorldService.GetMainMenuSceneVirtualWorld(actorModel.ActorLinkedToMainMenuScene.Id);
                }
            }
        }

        public List<ServerActor> GetServerActors()
        {
            return _entityManager.GetComponents<ServerActor>().ToList();
        }
    }
}
