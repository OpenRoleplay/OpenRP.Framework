using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private ILogger<ItemManager> _logger;
        private DateTime _lastUpdate;

        public ItemManager(
            BaseDataContext dataContext, 
            IEntityManager entityManager,
            ILogger<ItemManager> logger
        )
        {
            _dataContext = dataContext;
            _entityManager = entityManager;
            _logger = logger;

            // Change last update
            _lastUpdate = DateTime.UtcNow;
        }

        public void ProcessChanges()
        {
            DateTime changesSince = _lastUpdate;
            _lastUpdate = DateTime.Now;

            int itemsAdded = LoadNewItems(changesSince);
            int itemsUpdated = UpdateItems(changesSince);
        }

        public int LoadItems()
        {
            _logger.LogInformation("Begin loading items from database.");
            List<ItemModel> itemModels = _dataContext.Items
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

            _logger.LogInformation("Loaded {0} items.", amountLoaded);

            _logger.LogInformation("Finished loading items from database.");
            return amountLoaded;
        }

        private int LoadNewItems(DateTime changesSince)
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
