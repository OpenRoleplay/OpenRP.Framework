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

namespace OpenRP.Framework.Features.Inventories.Services
{
    public class InventoryItemManager : IInventoryItemManager
    {
        private BaseDataContext _dataContext;
        private IEntityManager _entityManager;
        private ILogger<InventoryItemManager> _logger;
        private DateTime _lastUpdate;

        public InventoryItemManager(
            BaseDataContext dataContext,
            IEntityManager entityManager,
            ILogger<InventoryItemManager> logger
        )
        {
            _dataContext = dataContext;
            _entityManager = entityManager;
            _logger = logger;

            // Change last update
            _lastUpdate = DateTime.UtcNow;
        }

        public async void ProcessChanges()
        {
            DateTime changesSince = _lastUpdate;
            _lastUpdate = DateTime.Now;

            int inventoriesAdded = LoadNewInventoryItems(changesSince);
            int inventoriesUpdated = UpdateInventoryItems(changesSince);
            int inventoriesSaved = await SaveInventoryItems();
            int inventoriesCreated = await CreateInventoryItems();
            int inventoriesDeleted = await DeleteInventoryItems();
        }

        public int LoadInventoryItems()
        {
            _logger.LogInformation("Begin loading inventory items from database.");
            List<InventoryItemModel> inventoryItemModels = _dataContext.InventoryItems
                .AsNoTracking()
                .ToList();

            int amountLoaded = 0;
            foreach (InventoryItemModel inventoryItemModel in inventoryItemModels)
            {
                EntityId inventoryEntityId = InventoryEntities.GetInventoryId((int)inventoryItemModel.InventoryId);
                EntityId inventoryItemEntityId = InventoryEntities.GetInventoryItemId((int)inventoryItemModel.Id);

                _entityManager.Create(inventoryItemEntityId, inventoryEntityId);

                InventoryItem inventoryItem = _entityManager.AddComponent<InventoryItem>(inventoryItemEntityId, inventoryItemModel);

                amountLoaded++;
            }

            _logger.LogInformation("Loaded {0} inventory items.", amountLoaded);

            _logger.LogInformation("Finished inventory items from database.");
            return amountLoaded;
        }

        private int LoadNewInventoryItems(DateTime changesSince)
        {
            List<InventoryItemModel> inventoryItemModels = _dataContext.InventoryItems
                .Where(i => i.CreatedOn > changesSince)
                .AsNoTracking()
                .ToList();

            int amountLoaded = 0;
            foreach (InventoryItemModel inventoryItemModel in inventoryItemModels)
            {
                EntityId inventoryEntityId = InventoryEntities.GetInventoryId((int)inventoryItemModel.InventoryId);
                EntityId inventoryItemEntityId = InventoryEntities.GetInventoryItemId((int)inventoryItemModel.Id);

                _entityManager.Create(inventoryItemEntityId, inventoryEntityId);

                InventoryItem inventoryItem = _entityManager.AddComponent<InventoryItem>(inventoryItemEntityId, inventoryItemModel);

                amountLoaded++;
            }

            return amountLoaded;
        }

        private int UpdateInventoryItems(DateTime changesSince)
        {
            List<InventoryItemModel> inventoryItemModels = _dataContext.InventoryItems
                .Where(i => i.UpdatedOn > changesSince)
                .AsNoTracking()
                .ToList();

            int amountLoaded = 0;
            foreach (InventoryItemModel inventoryItemModel in inventoryItemModels)
            {
                EntityId inventoryEntityId = InventoryEntities.GetInventoryId((int)inventoryItemModel.InventoryId);
                EntityId inventoryItemEntityId = InventoryEntities.GetInventoryItemId((int)inventoryItemModel.Id);

                _entityManager.Destroy(inventoryItemEntityId);
                _entityManager.Create(inventoryItemEntityId, inventoryEntityId);
                InventoryItem inventoryItem = _entityManager.AddComponent<InventoryItem>(inventoryItemEntityId, inventoryItemModel);

                amountLoaded++;
            }

            return amountLoaded;
        }

        private async Task<int> SaveInventoryItems()
        {
            int amountLoaded = 0;
            foreach (InventoryItem inventoryItem in _entityManager.GetComponents<InventoryItem>())
            {
                if (inventoryItem.HasChanges())
                {
                    InventoryItemModel inventoryItemModel = inventoryItem.GetRawInventoryItemModel();

                    if (inventoryItemModel != null)
                    {
                        _dataContext.InventoryItems.Update(inventoryItemModel);

                        if (await _dataContext.SaveChangesAsync() > 0)
                        {
                            inventoryItem.ProcessChanges(false);
                            amountLoaded++;
                        }
                    }
                }
            }

            return amountLoaded;
        }

        private async Task<int> CreateInventoryItems()
        {
            int amountCreated = 0;
            foreach (InventoryItem inventoryItem in _entityManager.GetComponents<InventoryItem>())
            {
                if (inventoryItem.GetId() == 0)
                {
                    InventoryItemModel inventoryItemModel = inventoryItem.GetRawInventoryItemModel();

                    if (inventoryItemModel != null)
                    {
                        var createdInventoryItemModel = _dataContext.InventoryItems.Update(inventoryItemModel);

                        if (await _dataContext.SaveChangesAsync() > 0)
                        {
                            inventoryItem.ProcessChanges(false);

                            inventoryItem.Destroy();

                            EntityId inventoryEntityId = InventoryEntities.GetInventoryId((int)createdInventoryItemModel.Entity.InventoryId);
                            EntityId inventoryItemEntityId = InventoryEntities.GetInventoryItemId((int)createdInventoryItemModel.Entity.Id);

                            _entityManager.Create(inventoryItemEntityId, inventoryEntityId);

                            InventoryItem newInventoryItem = _entityManager.AddComponent<InventoryItem>(inventoryItemEntityId, inventoryItemModel);

                            amountCreated++;
                        }
                    }
                }
            }

            InventoryNewEntities.ResetNewInventoryItemId();

            return amountCreated;
        }

        private async Task<int> DeleteInventoryItems()
        {
            int amountLoaded = 0;
            foreach (InventoryItem inventoryItem in _entityManager.GetComponents<InventoryItem>())
            {
                if (inventoryItem.IsDeleted())
                {
                    InventoryItemModel inventoryItemModel = inventoryItem.GetRawInventoryItemModel();

                    if (inventoryItemModel != null)
                    {
                        _dataContext.InventoryItems.Remove(inventoryItemModel);

                        if (await _dataContext.SaveChangesAsync() > 0)
                        {
                            inventoryItem.ProcessChanges(false);
                            amountLoaded++;
                        }
                    }
                }
            }

            return amountLoaded;
        }
    }
}
