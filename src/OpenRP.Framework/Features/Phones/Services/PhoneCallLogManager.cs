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
    public class PhoneCallLogManager : BaseManager<PhoneCallLogModel, PhoneCallLog>, IPhoneCallLogManager
    {
        public PhoneCallLogManager(BaseDataContext dataContext, IEntityManager entityManager, ILogger<PhoneCallLogManager> logger)
            : base(dataContext, entityManager, logger)
        {
        }

        protected override DbSet<PhoneCallLogModel> GetDbSet() => _dataContext.PhoneCallLogs;

        protected override Guid EntityTypeKey => PhoneEntities.PhoneCallLogType;

        protected override PhoneCallLogModel GetModelFromComponent(PhoneCallLog component)
        {
            return component.GetRawPhoneCallLogModel();
        }
    }
}
