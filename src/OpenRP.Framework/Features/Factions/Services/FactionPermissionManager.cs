using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Factions.Entities;
using OpenRP.Framework.Shared.BaseManager.Helpers;
using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.Factions.Services
{
    public class FactionPermissionManager : BaseManager<FactionPermissionModel, FactionPermission>, IFactionPermissionManager
    {
        public FactionPermissionManager(BaseDataContext dataContext, IEntityManager entityManager, ILogger<FactionPermissionManager> logger)
            : base(dataContext, entityManager, logger) { }

        protected override DbSet<FactionPermissionModel> GetDbSet() => _dataContext.FactionPermissions;

        protected override Guid EntityTypeKey => FactionEntities.FactionPermissionType;

        protected override EntityId? GetParentEntityId(FactionPermissionModel model)
        {
            return FactionEntities.GetFactionId((int)model.FactionId);
        }

        protected override FactionPermissionModel GetModelFromComponent(FactionPermission component)
        {
            return component.GetRawFactionPermissionModel();
        }
    }
}
