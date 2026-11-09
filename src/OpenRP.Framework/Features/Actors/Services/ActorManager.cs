using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Actors.Entities;
using OpenRP.Framework.Shared.BaseManager.Helpers;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Actors.Services
{
    /// <summary>
    /// Loads, saves and syncs the Actors table.
    /// </summary>
    public class ActorManager : BaseManager<ActorModel, ServerActor>, IActorManager
    {
        private readonly IActorService _actorService;

        public ActorManager(BaseDataContext dataContext, IEntityManager entityManager, IActorService actorService, ILogger<ActorManager> logger)
            : base(dataContext, entityManager, logger)
        {
            _actorService = actorService;
        }

        protected override DbSet<ActorModel> GetDbSet() => _dataContext.Actors;

        protected override Guid EntityTypeKey => ActorEntities.ServerActorType;

        protected override ActorModel GetModelFromComponent(ServerActor component)
        {
            return component.GetRawActorModel();
        }

        /// <summary>
        /// (Re)creates the game actor and its label, so a change made on another server shows up after the sync.
        /// </summary>
        protected override void OnComponentLoaded(ServerActor component)
        {
            _actorService.CreateActorInWorld(component);
        }
    }
}
