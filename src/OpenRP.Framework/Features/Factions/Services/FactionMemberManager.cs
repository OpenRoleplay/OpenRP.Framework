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
    public class FactionMemberManager : BaseManager<FactionMemberModel, FactionMember>, IFactionMemberManager
    {
        public FactionMemberManager(BaseDataContext dataContext, IEntityManager entityManager, ILogger<FactionMemberManager> logger)
            : base(dataContext, entityManager, logger) { }

        protected override DbSet<FactionMemberModel> GetDbSet() => _dataContext.FactionMembers;

        protected override Guid EntityTypeKey => FactionEntities.FactionMemberType;

        protected override EntityId? GetParentEntityId(FactionMemberModel model)
        {
            return FactionEntities.GetFactionId((int)model.FactionId);
        }

        protected override FactionMemberModel GetModelFromComponent(FactionMember component)
        {
            return component.GetRawFactionMemberModel();
        }
    }
}
