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
    public class RadioFrequencyManager : BaseManager<RadioFrequencyModel, RadioFrequency>, IRadioFrequencyManager
    {
        public RadioFrequencyManager(BaseDataContext dataContext, IEntityManager entityManager, ILogger<RadioFrequencyManager> logger)
            : base(dataContext, entityManager, logger)
        {
        }

        protected override DbSet<RadioFrequencyModel> GetDbSet() => _dataContext.RadioFrequencies;

        protected override Guid EntityTypeKey => RadioEntities.RadioFrequencyType;

        protected override RadioFrequencyModel GetModelFromComponent(RadioFrequency component)
        {
            return component.GetRawRadioFrequencyModel();
        }
    }
}
