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
    public class FactionTaxRegistrationManager : BaseManager<FactionTaxRegistrationModel, FactionTaxRegistration>, IFactionTaxRegistrationManager
    {
        public FactionTaxRegistrationManager(BaseDataContext dataContext, IEntityManager entityManager, ILogger<FactionTaxRegistrationManager> logger)
            : base(dataContext, entityManager, logger)
        {
        }

        protected override DbSet<FactionTaxRegistrationModel> GetDbSet() => _dataContext.FactionTaxRegistrations;

        protected override Guid EntityTypeKey => JurisdictionEntities.FactionTaxRegistrationType;

        protected override FactionTaxRegistrationModel GetModelFromComponent(FactionTaxRegistration component)
        {
            return component.GetRawFactionTaxRegistrationModel();
        }
    }
}
