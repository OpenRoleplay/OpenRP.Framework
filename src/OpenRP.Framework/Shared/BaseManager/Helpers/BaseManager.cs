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

namespace OpenRP.Framework.Shared.BaseManager.Helpers
{
    /// <summary>
    /// Keeps the components of one table in sync with the database: loads everything at startup, picks up rows
    /// that other code wrote to the database, and saves, creates and deletes changed components.
    /// </summary>
    public abstract class BaseManager<TModel, TComponent>
        where TModel : BaseModel, new()
        where TComponent : Component
    {
        protected BaseDataContext _dataContext;
        protected IEntityManager _entityManager;
        protected ILogger _logger;

        // UTC, because BaseDataContext stores CreatedOn and UpdatedOn in UTC.
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
            try
            {
                DateTime changesSince = _lastUpdate;
                _lastUpdate = DateTime.UtcNow;

                LoadNew(changesSince);
                Update(changesSince);
                await SaveAsync();
                await CreateAsync();
                await DeleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process changes for {ModelName}.", typeof(TModel).Name);
            }
        }

        protected abstract DbSet<TModel> GetDbSet();
        protected abstract EntityId GetEntityId(TModel model);
        protected abstract TModel GetModelFromComponent(TComponent component);
        protected abstract void ResetNewId();
        protected virtual EntityId? GetParentEntityId(TModel model) => null;

        public int LoadAll()
        {
            _logger.LogInformation($"Begin loading all {typeof(TModel).Name} from database.");
            List<TModel> models = GetDbSet().AsNoTracking().ToList();
            foreach (TModel model in models)
            {
                CreateEntityWithComponent(model);
            }
            _logger.LogInformation($"Loaded {models.Count} {typeof(TModel).Name}.");
            return models.Count;
        }

        /// <summary>
        /// Loads rows that other code inserted into the database. Rows this manager created itself are already loaded.
        /// </summary>
        protected int LoadNew(DateTime changesSince)
        {
            int count = 0;
            foreach (TModel model in GetModelsCreatedSince(changesSince))
            {
                if (IsEntityLoaded(GetEntityId(model)))
                {
                    continue;
                }

                CreateEntityWithComponent(model);
                count++;
            }
            return count;
        }

        /// <summary>
        /// Reloads rows that other code changed in the database. Components with unsaved changes keep their own state.
        /// </summary>
        protected int Update(DateTime changesSince)
        {
            int count = 0;
            foreach (TModel model in GetModelsUpdatedSince(changesSince))
            {
                EntityId entityId = GetEntityId(model);
                if (HasUnsavedChanges(entityId) || IsComponentUpToDate(entityId, model))
                {
                    continue;
                }

                ReloadEntity(entityId, model);
                count++;
            }
            return count;
        }

        protected async Task<int> SaveAsync()
        {
            int count = 0;
            foreach (TComponent component in _entityManager.GetComponents<TComponent>().ToList())
            {
                if (component is not IChangeable changeable || !changeable.HasChanges())
                {
                    continue;
                }

                if (await SaveModelAsync(GetModelFromComponent(component)))
                {
                    changeable.ProcessChanges(false);
                    count++;
                }
            }
            return count;
        }

        protected async Task<int> CreateAsync()
        {
            int count = 0;
            foreach (TComponent component in _entityManager.GetComponents<TComponent>().ToList())
            {
                if (component is not IBaseDataComponent dataComponent || dataComponent.GetId() != 0)
                {
                    continue;
                }

                TModel model = GetModelFromComponent(component);
                if (await SaveModelAsync(model))
                {
                    RecreateEntityWithNewId(component, model);
                    count++;
                }
            }
            ResetNewId();
            return count;
        }

        protected async Task<int> DeleteAsync()
        {
            int count = 0;
            foreach (TComponent component in _entityManager.GetComponents<TComponent>().ToList())
            {
                if (component is not IDeletable deletable || !deletable.IsDeleted())
                {
                    continue;
                }

                TModel model = GetModelFromComponent(component);
                if (model.Id != 0 && await DeleteModelAsync(model))
                {
                    deletable.ProcessDeletion(false);
                    if (component is IChangeable changeable)
                    {
                        changeable.ProcessChanges(false);
                    }
                    count++;
                }
            }
            return count;
        }

        private List<TModel> GetModelsCreatedSince(DateTime changesSince)
        {
            return GetDbSet().Where(m => m.CreatedOn > changesSince).AsNoTracking().ToList();
        }

        private List<TModel> GetModelsUpdatedSince(DateTime changesSince)
        {
            return GetDbSet().Where(m => m.UpdatedOn > changesSince).AsNoTracking().ToList();
        }

        private bool IsEntityLoaded(EntityId entityId)
        {
            return _entityManager.Exists(entityId);
        }

        private bool HasUnsavedChanges(EntityId entityId)
        {
            if (!IsEntityLoaded(entityId))
            {
                return false;
            }

            TComponent? component = _entityManager.GetComponent<TComponent>(entityId);
            bool hasChanges = component is IChangeable changeable && changeable.HasChanges();
            bool isDeleted = component is IDeletable deletable && deletable.IsDeleted();
            return hasChanges || isDeleted;
        }

        /// <summary>
        /// True when the loaded component already has this version of the row, for example because this manager saved it.
        /// </summary>
        private bool IsComponentUpToDate(EntityId entityId, TModel model)
        {
            if (!IsEntityLoaded(entityId))
            {
                return false;
            }

            TComponent? component = _entityManager.GetComponent<TComponent>(entityId);
            return component != null && GetModelFromComponent(component).UpdatedOn >= model.UpdatedOn;
        }

        private bool HasParentChanged(EntityId entityId, TModel model)
        {
            EntityId? newParentId = GetParentEntityId(model);
            return newParentId.HasValue && _entityManager.GetParent(entityId) != newParentId.Value;
        }

        /// <summary>
        /// Swaps the component for one built from the new model. The entity itself is kept when possible,
        /// so its child entities (for example the items of an inventory) stay loaded.
        /// </summary>
        private void ReloadEntity(EntityId entityId, TModel model)
        {
            if (!IsEntityLoaded(entityId))
            {
                CreateEntityWithComponent(model);
                return;
            }

            if (HasParentChanged(entityId, model))
            {
                _entityManager.Destroy(entityId);
                CreateEntityWithComponent(model);
                return;
            }

            _entityManager.Destroy<TComponent>(entityId);
            _entityManager.AddComponent<TComponent>(entityId, model);
        }

        private void CreateEntityWithComponent(TModel model)
        {
            EntityId entityId = GetEntityId(model);
            EntityId? parentId = GetParentEntityId(model);
            if (parentId.HasValue)
            {
                _entityManager.Create(entityId, parentId.Value);
            }
            else
            {
                _entityManager.Create(entityId);
            }
            _entityManager.AddComponent<TComponent>(entityId, model);
        }

        /// <summary>
        /// A new component gets its database id on its first save, so it moves to the entity matching that id.
        /// </summary>
        private void RecreateEntityWithNewId(TComponent component, TModel savedModel)
        {
            if (component is IChangeable changeable)
            {
                changeable.ProcessChanges(false);
            }

            component.DestroyEntity();
            CreateEntityWithComponent(savedModel);
        }

        /// <summary>
        /// Inserts or updates the model. It is detached afterwards, because the component keeps the instance and
        /// a reloaded copy of the same row would otherwise clash with it in this long-lived context.
        /// </summary>
        private async Task<bool> SaveModelAsync(TModel model)
        {
            try
            {
                _dataContext.Update(model);
                return await _dataContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save {ModelName} {ModelId}.", typeof(TModel).Name, model.Id);
                return false;
            }
            finally
            {
                _dataContext.Entry(model).State = EntityState.Detached;
            }
        }

        private async Task<bool> DeleteModelAsync(TModel model)
        {
            try
            {
                _dataContext.Remove(model);
                return await _dataContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete {ModelName} {ModelId}.", typeof(TModel).Name, model.Id);
                return false;
            }
            finally
            {
                _dataContext.Entry(model).State = EntityState.Detached;
            }
        }
    }
}
