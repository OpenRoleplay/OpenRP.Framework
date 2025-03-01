using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Entities;
using OpenRP.Framework.Database;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Shared.BaseManager.Entities;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Inventories.Entities;

namespace OpenRP.Framework.Shared.BaseManager.Helpers
{
    public abstract class BaseManager<TModel, TComponent>
        where TModel : BaseModel, new()
        where TComponent : Component
    {
        protected BaseDataContext _dataContext;
        protected IEntityManager _entityManager;
        protected ILogger _logger;
        protected DateTime _lastUpdate;

        protected BaseManager(BaseDataContext dataContext, IEntityManager entityManager, ILogger logger)
        {
            _dataContext = dataContext;
            _entityManager = entityManager;
            _logger = logger;
            _lastUpdate = DateTime.UtcNow;
        }

        public async void ProcessChanges()
        {
            DateTime changesSince = _lastUpdate;
            _lastUpdate = DateTime.Now;

            int added = LoadNew(changesSince);
            int updated = Update(changesSince);
            int saved = await SaveAsync();
            int created = await CreateAsync();
            int deleted = await DeleteAsync();
        }

        protected abstract DbSet<TModel> GetDbSet();
        protected abstract EntityId GetEntityId(TModel model);
        protected abstract TModel GetModelFromComponent(TComponent component);
        protected abstract void ResetNewId();

        public int LoadAll()
        {
            _logger.LogInformation($"Begin loading all {typeof(TModel).Name} from database.");
            var models = GetDbSet().AsNoTracking().ToList();
            int count = 0;
            foreach (var model in models)
            {
                EntityId entityId = GetEntityId(model);
                _entityManager.Create(entityId);
                _entityManager.AddComponent<TComponent>(entityId, model);
                count++;
            }
            _logger.LogInformation($"Loaded {count} {typeof(TModel).Name}.");
            return count;
        }

        protected int LoadNew(DateTime changesSince)
        {
            var models = GetDbSet().Where(m => m.CreatedOn > changesSince)
                                   .AsNoTracking()
                                   .ToList();
            int count = 0;
            foreach (var model in models)
            {
                EntityId entityId = GetEntityId(model);
                _entityManager.Create(entityId);
                _entityManager.AddComponent<TComponent>(entityId, model);
                count++;
            }
            return count;
        }

        protected int Update(DateTime changesSince)
        {
            var models = GetDbSet().Where(m => m.UpdatedOn > changesSince)
                                   .AsNoTracking()
                                   .ToList();
            int count = 0;
            foreach (var model in models)
            {
                EntityId entityId = GetEntityId(model);
                _entityManager.Destroy(entityId);
                _entityManager.Create(entityId);
                _entityManager.AddComponent<TComponent>(entityId, model);
                count++;
            }
            return count;
        }

        protected async Task<int> SaveAsync()
        {
            int count = 0;
            foreach (var component in _entityManager.GetComponents<TComponent>())
            {
                // Assume each TComponent implements an interface for change tracking.
                if (component is IChangeable changeTrackable && changeTrackable.HasChanges())
                {
                    TModel model = GetModelFromComponent(component);
                    _dataContext.Update(model);
                    if (await _dataContext.SaveChangesAsync() > 0)
                    {
                        changeTrackable.ProcessChanges(false);
                        count++;
                    }
                }
            }
            return count;
        }

        protected async Task<int> CreateAsync()
        {
            int count = 0;
            foreach (var component in _entityManager.GetComponents<TComponent>())
            {
                if (component is IBaseDataComponent dataComponent && dataComponent.GetId() == 0)
                {
                    TModel model = GetModelFromComponent(component);
                    var updatedModel = _dataContext.Update(model).Entity;
                    if (await _dataContext.SaveChangesAsync() > 0)
                    {
                        if (component is IChangeable changeTrackable)
                            changeTrackable.ProcessChanges(false);

                        // Recreate the component with its new id.
                        EntityId oldEntityId = GetEntityId(model);
                        _entityManager.Destroy(oldEntityId);
                        EntityId newEntityId = GetEntityId(updatedModel);
                        _entityManager.Create(newEntityId);
                        _entityManager.AddComponent<TComponent>(newEntityId, model);
                        count++;
                    }
                }
            }
            ResetNewId();
            return count;
        }

        protected async Task<int> DeleteAsync()
        {
            int count = 0;
            foreach (var component in _entityManager.GetComponents<TComponent>())
            {
                // Assume TComponent implements IDeletable for deletion state.
                if (component is IDeletable deletable && deletable.IsDeleted())
                {
                    TModel model = GetModelFromComponent(component);
                    _dataContext.Remove(model);
                    if (await _dataContext.SaveChangesAsync() > 0)
                    {
                        if (component is IChangeable changeTrackable)
                            changeTrackable.ProcessChanges(false);
                        count++;
                    }
                }
            }
            return count;
        }
    }

}
