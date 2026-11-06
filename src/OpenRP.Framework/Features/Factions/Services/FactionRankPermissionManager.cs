using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Factions.Entities;
using OpenRP.Framework.Shared.BaseManager.Helpers;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Factions.Services
{
    public class FactionRankPermissionManager : BaseManager<FactionRankPermissionModel, FactionRankPermission>, IFactionRankPermissionManager
    {
        public FactionRankPermissionManager(BaseDataContext dataContext, IEntityManager entityManager, ILogger<FactionRankPermissionManager> logger)
            : base(dataContext, entityManager, logger) { }

        protected override DbSet<FactionRankPermissionModel> GetDbSet() => _dataContext.FactionRankPermissions;

        protected override EntityId GetEntityId(FactionRankPermissionModel model)
        {
            return FactionEntities.GetFactionRankPermissionId((int)model.Id);
        }

        protected override EntityId? GetParentEntityId(FactionRankPermissionModel model)
        {
            return FactionEntities.GetFactionRankId((int)model.FactionRankId);
        }

        protected override FactionRankPermissionModel GetModelFromComponent(FactionRankPermission component)
        {
            return component.GetRawFactionRankPermissionModel();
        }
    }
}
