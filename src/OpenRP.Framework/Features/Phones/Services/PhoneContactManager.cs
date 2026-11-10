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
    public class PhoneContactManager : BaseManager<PhoneContactModel, PhoneContact>, IPhoneContactManager
    {
        public PhoneContactManager(BaseDataContext dataContext, IEntityManager entityManager, ILogger<PhoneContactManager> logger)
            : base(dataContext, entityManager, logger)
        {
        }

        protected override DbSet<PhoneContactModel> GetDbSet() => _dataContext.PhoneContacts;

        protected override Guid EntityTypeKey => PhoneEntities.PhoneContactType;

        protected override PhoneContactModel GetModelFromComponent(PhoneContact component)
        {
            return component.GetRawPhoneContactModel();
        }
    }
}
