using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Database;
using OpenRP.Framework.Features.Items.Components;
using OpenRP.Framework.Features.Items.Entities;
using OpenRP.Framework.Features.Items.Services;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Features.Inventories.Entities;
using OpenRP.Framework.Features.Inventories.Components;
using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Shared.BaseManager.Helpers;
using Microsoft.Extensions.Logging;

namespace OpenRP.Framework.Features.Inventories.Services
{
    public class InventoryManager : BaseManager<InventoryModel, Inventory>, IBaseManager
    {
        public InventoryManager(BaseDataContext dataContext, IEntityManager entityManager, ILogger<InventoryManager> logger)
            : base(dataContext, entityManager, logger) { }

        protected override DbSet<InventoryModel> GetDbSet() => _dataContext.Inventories;

        protected override EntityId GetEntityId(InventoryModel model)
        {
            return InventoryEntities.GetInventoryId((int)model.Id);
        }

        protected override InventoryModel GetModelFromComponent(Inventory component)
        {
            return component.GetRawInventoryModel();
        }

        protected override void ResetNewId()
        {
            InventoryNewEntities.ResetNewInventoryId();
        }
    }
}
