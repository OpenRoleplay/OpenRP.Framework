using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Actors.Entities;
using OpenRP.Framework.Features.Items.Components;
using OpenRP.Framework.Features.Items.Entities;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Items.Services
{
    public class ItemManager : IItemManager
    {
        private BaseDataContext _dataContext;
        private IEntityManager _entityManager;
        private DateTime _lastUpdate;

        public ItemManager(
            BaseDataContext dataContext, 
            IEntityManager entityManager
        )
        {
            _dataContext = dataContext;
            _entityManager = entityManager;
        }

        public void ProcessChanges()
        {
            DateTime changesSince = _lastUpdate;
            _lastUpdate = DateTime.Now;

            int itemsAdded = LoadItems(changesSince);
            int itemsUpdated = UpdateItems(changesSince);
        }

        private int LoadItems(DateTime changesSince)
        {
            List<ItemModel> itemModels = _dataContext.Items
                .Where(i => i.CreatedOn > changesSince)
                .AsNoTracking()
                .ToList();

            int amountLoaded = 0;
            foreach (ItemModel itemModel in itemModels)
            {
                EntityId itemEntityId = ItemEntities.GetItemId((int)itemModel.Id);
                _entityManager.Create(itemEntityId);

                Item item = _entityManager.AddComponent<Item>(itemEntityId, itemModel);

                amountLoaded++;
            }

            return amountLoaded;
        }

        private int UpdateItems(DateTime changesSince)
        {
            List<ItemModel> itemModels = _dataContext.Items
                .Where(i => i.UpdatedOn > changesSince)
                .AsNoTracking()
                .ToList();

            int amountLoaded = 0;
            foreach (ItemModel itemModel in itemModels)
            {
                EntityId itemEntityId = ItemEntities.GetItemId((int)itemModel.Id);

                _entityManager.Destroy(itemEntityId);
                Item item = _entityManager.AddComponent<Item>(itemEntityId, itemModel);

                amountLoaded++;
            }

            return amountLoaded;
        }
    }
}
