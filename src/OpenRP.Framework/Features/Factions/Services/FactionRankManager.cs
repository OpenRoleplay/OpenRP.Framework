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
    /// <summary>
    /// Ranks are child entities of their faction. They aren't nested under their parent rank: a sub-rank can be
    /// loaded before its parent rank, so the rank tree is looked up by id instead.
    /// </summary>
    public class FactionRankManager : BaseManager<FactionRankModel, FactionRank>, IFactionRankManager
    {
        public FactionRankManager(BaseDataContext dataContext, IEntityManager entityManager, ILogger<FactionRankManager> logger)
            : base(dataContext, entityManager, logger) { }

        protected override DbSet<FactionRankModel> GetDbSet() => _dataContext.FactionRanks;

        protected override EntityId GetEntityId(FactionRankModel model)
        {
            return FactionEntities.GetFactionRankId((int)model.Id);
        }

        protected override EntityId? GetParentEntityId(FactionRankModel model)
        {
            return FactionEntities.GetFactionId((int)model.FactionId);
        }

        protected override FactionRankModel GetModelFromComponent(FactionRank component)
        {
            return component.GetRawFactionRankModel();
        }
    }
}
