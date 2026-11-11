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
    public class RadioManager : BaseManager<RadioModel, Radio>, IRadioManager
    {
        public RadioManager(BaseDataContext dataContext, IEntityManager entityManager, ILogger<RadioManager> logger)
            : base(dataContext, entityManager, logger)
        {
        }

        protected override DbSet<RadioModel> GetDbSet() => _dataContext.Radios;

        protected override Guid EntityTypeKey => RadioEntities.RadioType;

        protected override RadioModel GetModelFromComponent(Radio component)
        {
            return component.GetRawRadioModel();
        }
    }
}
