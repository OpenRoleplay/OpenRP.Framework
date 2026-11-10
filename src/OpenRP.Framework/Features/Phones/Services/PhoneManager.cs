using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Phones.Components;
using OpenRP.Framework.Features.Phones.Entities;
using OpenRP.Framework.Shared.BaseManager.Helpers;
using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.Phones.Services
{
    public class PhoneManager : BaseManager<PhoneModel, Phone>, IPhoneManager
    {
        public PhoneManager(BaseDataContext dataContext, IEntityManager entityManager, ILogger<PhoneManager> logger)
            : base(dataContext, entityManager, logger)
        {
        }

        protected override DbSet<PhoneModel> GetDbSet() => _dataContext.Phones;

        protected override Guid EntityTypeKey => PhoneEntities.PhoneType;

        protected override PhoneModel GetModelFromComponent(Phone component)
        {
            return component.GetRawPhoneModel();
        }
    }
}
