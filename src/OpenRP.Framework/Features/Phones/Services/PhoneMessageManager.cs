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
    public class PhoneMessageManager : BaseManager<PhoneMessageModel, PhoneMessage>, IPhoneMessageManager
    {
        public PhoneMessageManager(BaseDataContext dataContext, IEntityManager entityManager, ILogger<PhoneMessageManager> logger)
            : base(dataContext, entityManager, logger)
        {
        }

        protected override DbSet<PhoneMessageModel> GetDbSet() => _dataContext.PhoneMessages;

        protected override Guid EntityTypeKey => PhoneEntities.PhoneMessageType;

        protected override PhoneMessageModel GetModelFromComponent(PhoneMessage component)
        {
            return component.GetRawPhoneMessageModel();
        }
    }
}
