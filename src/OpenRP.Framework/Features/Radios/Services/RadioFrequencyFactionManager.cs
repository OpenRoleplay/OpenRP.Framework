using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Radios.Components;
using OpenRP.Framework.Features.Radios.Entities;
using OpenRP.Framework.Shared.BaseManager.Helpers;
using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.Radios.Services
{
    public class RadioFrequencyFactionManager : BaseManager<RadioFrequencyFactionModel, RadioFrequencyFaction>, IRadioFrequencyFactionManager
    {
        public RadioFrequencyFactionManager(BaseDataContext dataContext, IEntityManager entityManager, ILogger<RadioFrequencyFactionManager> logger)
            : base(dataContext, entityManager, logger)
        {
        }

        protected override DbSet<RadioFrequencyFactionModel> GetDbSet() => _dataContext.RadioFrequencyFactions;

        protected override Guid EntityTypeKey => RadioEntities.RadioFrequencyFactionType;

        protected override RadioFrequencyFactionModel GetModelFromComponent(RadioFrequencyFaction component)
        {
            return component.GetRawRadioFrequencyFactionModel();
        }
    }
}
