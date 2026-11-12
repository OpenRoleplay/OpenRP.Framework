using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Actors.Entities;
using OpenRP.Framework.Shared.BaseManager.Helpers;
using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.Actors.Services
{
    public class ActorPermissionManager : BaseManager<ActorPermissionModel, ActorPermission>, IActorPermissionManager
    {
        public ActorPermissionManager(BaseDataContext dataContext, IEntityManager entityManager, ILogger<ActorPermissionManager> logger)
            : base(dataContext, entityManager, logger)
        {
        }

        protected override DbSet<ActorPermissionModel> GetDbSet() => _dataContext.ActorPermissions;

        protected override Guid EntityTypeKey => ActorEntities.ActorPermissionType;

        protected override ActorPermissionModel GetModelFromComponent(ActorPermission component)
        {
            return component.GetRawActorPermissionModel();
        }
    }
}
