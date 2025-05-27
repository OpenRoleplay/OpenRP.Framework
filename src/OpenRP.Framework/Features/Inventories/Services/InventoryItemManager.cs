using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Database;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Inventories.Entities;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Shared.BaseManager.Helpers;

namespace OpenRP.Framework.Features.Inventories.Services
{
    public class InventoryItemManager : BaseManager<InventoryItemModel, InventoryItem>, IInventoryItemManager
    {
        public InventoryItemManager(BaseDataContext dataContext, IEntityManager entityManager, ILogger<InventoryItemManager> logger)
            : base(dataContext, entityManager, logger) { }

        protected override DbSet<InventoryItemModel> GetDbSet() => _dataContext.InventoryItems;

        protected override EntityId GetEntityId(InventoryItemModel model)
        {
            return InventoryEntities.GetInventoryItemId((int)model.Id);
        }

        protected override EntityId? GetParentEntityId(InventoryItemModel model)
        {
            return InventoryEntities.GetInventoryId((int)model.InventoryId);
        }

        protected override InventoryItemModel GetModelFromComponent(InventoryItem component)
        {
            return component.GetRawInventoryItemModel();
        }

        protected override void ResetNewId()
        {
            InventoryNewEntities.ResetNewInventoryItemId();
        }
    }
}
