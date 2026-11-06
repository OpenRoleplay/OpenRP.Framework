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
    /// Factions are not child entities of their parent faction: a sub-faction can be loaded before its parent, and
    /// an entity can't be created under a parent that doesn't exist yet. The parent is looked up by id instead.
    /// </summary>
    public class FactionManager : BaseManager<FactionModel, Faction>, IFactionManager
    {
        public FactionManager(BaseDataContext dataContext, IEntityManager entityManager, ILogger<FactionManager> logger)
            : base(dataContext, entityManager, logger) { }

        protected override DbSet<FactionModel> GetDbSet() => _dataContext.Factions;

        protected override EntityId GetEntityId(FactionModel model)
        {
            return FactionEntities.GetFactionId((int)model.Id);
        }

        protected override FactionModel GetModelFromComponent(Faction component)
        {
            return component.GetRawFactionModel();
        }
    }
}
