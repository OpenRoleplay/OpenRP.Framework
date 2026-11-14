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
    public class JurisdictionSettingManager : BaseManager<JurisdictionSettingModel, JurisdictionSetting>, IJurisdictionSettingManager
    {
        public JurisdictionSettingManager(BaseDataContext dataContext, IEntityManager entityManager, ILogger<JurisdictionSettingManager> logger)
            : base(dataContext, entityManager, logger)
        {
        }

        protected override DbSet<JurisdictionSettingModel> GetDbSet() => _dataContext.JurisdictionSettings;

        protected override Guid EntityTypeKey => JurisdictionEntities.JurisdictionSettingType;

        protected override JurisdictionSettingModel GetModelFromComponent(JurisdictionSetting component)
        {
            return component.GetRawJurisdictionSettingModel();
        }
    }
}
