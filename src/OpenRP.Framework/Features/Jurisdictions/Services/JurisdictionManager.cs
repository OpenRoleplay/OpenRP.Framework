using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Jurisdictions.Components;
using OpenRP.Framework.Features.Jurisdictions.Entities;
using OpenRP.Framework.Shared.BaseManager.Helpers;
using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.Jurisdictions.Services
{
    public class JurisdictionManager : BaseManager<JurisdictionModel, Jurisdiction>, IJurisdictionManager
    {
        public JurisdictionManager(BaseDataContext dataContext, IEntityManager entityManager, ILogger<JurisdictionManager> logger)
            : base(dataContext, entityManager, logger)
        {
        }

        protected override DbSet<JurisdictionModel> GetDbSet() => _dataContext.Jurisdictions;

        protected override Guid EntityTypeKey => JurisdictionEntities.JurisdictionType;

        protected override JurisdictionModel GetModelFromComponent(Jurisdiction component)
        {
            return component.GetRawJurisdictionModel();
        }
    }
}
