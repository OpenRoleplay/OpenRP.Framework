using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Entities;
using OpenRP.Framework.Database;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenRP.Framework.Shared.BaseManager.Entities;
using Microsoft.Extensions.Logging;

namespace OpenRP.Framework.Shared.BaseManager.Helpers
{
    /// <summary>
    /// Keeps the components of one table in sync with the database. Components in memory are the live state:
    /// the manager loads everything at startup, saves what changed, and picks up rows that other code or other
    /// servers wrote to the same database. When two servers change the same row, the last save wins.
    /// </summary>
    /// <remarks>
    /// Everything runs synchronously on the main thread, so components are never touched from another thread and
    /// a shutdown save can't overlap a running cycle on the same database context.
    /// </remarks>
    public abstract class BaseManager<TModel, TComponent>
        where TModel : BaseModel, new()
        where TComponent : Component, IBaseDataComponent, IChangeable
    {
        // Pulling a little further back than the newest change seen covers small clock differences between servers;
        // it's harmless because components that are already up to date are skipped.
        private static readonly TimeSpan SyncOverlap = TimeSpan.FromSeconds(5);

        protected BaseDataContext _dataContext;
        protected IEntityManager _entityManager;
        protected ILogger _logger;

        // The newest CreatedOn/UpdatedOn seen in the database, in the database's (UTC) timestamps.
        private DateTime _newestChangeSeen = DateTime.MinValue;

        protected BaseManager(BaseDataContext dataContext, IEntityManager entityManager, ILogger logger)
        {
            _dataContext = dataContext;
            _entityManager = entityManager;
            _logger = logger;
        }

        protected abstract DbSet<TModel> GetDbSet();
        protected abstract EntityId GetEntityId(TModel model);
        protected abstract TModel GetModelFromComponent(TComponent component);
        protected virtual EntityId? GetParentEntityId(TModel model) => null;

        public int LoadAll()
        {
            _logger.LogInformation($"Begin loading all {typeof(TModel).Name} from database.");
            List<TModel> models = GetDbSet().AsNoTracking().ToList();
            foreach (TModel model in models)
            {
                CreateEntityWithComponent(model);
            }
            RememberNewestChange(models);
            _logger.LogInformation($"Loaded {models.Count} {typeof(TModel).Name}.");
            return models.Count;
        }

        /// <summary>
        /// Saves this server's changes, then picks up the changes of others. Called on a timer.
        /// </summary>
        public void ProcessChanges()
        {
            try
            {
                SaveNow();
                PullExternalChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process changes for {ModelName}.", typeof(TModel).Name);
            }
        }

        /// <summary>
        /// Saves changed components, inserts new ones and deletes marked ones, right now. Called on shutdown too.
        /// </summary>
        public void SaveNow()
        {
            SaveChangedComponents();
            CreateNewComponents();
            DeleteMarkedComponents();
        }

        /// <summary>
        /// Inserts the row right away and loads its component, for when the real id is needed immediately.
        /// </summary>
        public TComponent CreateAndLoad(TModel model)
        {
            try
            {
                _dataContext.Add(model);
                _dataContext.SaveChanges();
            }
            finally
            {
                _dataContext.ChangeTracker.Clear();
            }
            return CreateEntityWithComponent(model);
        }

        /// <summary>
        /// The component of this row, loading it from the database first when it isn't loaded yet. Null when the
        /// row doesn't exist.
        /// </summary>
        public TComponent? LoadById(ulong id)
        {
            TComponent? loadedComponent = GetLoadedComponent(id);
            if (loadedComponent != null)
            {
                return loadedComponent;
            }

            TModel? model = GetDbSet().AsNoTracking().FirstOrDefault(m => m.Id == id);
            return model == null ? null : CreateEntityWithComponent(model);
        }

        private TComponent? GetLoadedComponent(ulong id)
        {
            return GetComponents().FirstOrDefault(component => component.GetId() == id);
        }

        private List<TComponent> GetComponents()
        {
            return _entityManager.GetComponents<TComponent>().ToList();
        }

        private static bool IsMarkedDeleted(TComponent component)
        {
            return component is IDeletable deletable && deletable.IsDeleted();
        }

        private static bool IsUnsaved(TComponent component)
        {
            return component.GetId() == 0;
        }

        private void SaveChangedComponents()
        {
            List<TComponent> changedComponents = GetComponents()
                .Where(component => component.HasChanges() && !IsUnsaved(component) && !IsMarkedDeleted(component))
                .ToList();

            foreach (TComponent component in SaveComponents(changedComponents))
            {
                component.ProcessChanges(false);
            }
        }

        private void CreateNewComponents()
        {
            List<TComponent> newComponents = GetComponents()
                .Where(component => IsUnsaved(component) && !IsMarkedDeleted(component))
                .ToList();

            foreach (TComponent component in SaveComponents(newComponents))
            {
                RecreateEntityWithNewId(component);
            }
        }

        private void DeleteMarkedComponents()
        {
            List<TComponent> markedComponents = GetComponents().Where(IsMarkedDeleted).ToList();
            foreach (TComponent component in markedComponents.Where(IsUnsaved))
            {
                component.DestroyEntity();
            }

            foreach (TComponent component in DeleteComponents(markedComponents.Where(component => !IsUnsaved(component)).ToList()))
            {
                component.DestroyEntity();
            }
        }

        /// <summary>
        /// Inserts or updates the components' rows in one go. When that fails, they are saved one by one, so one bad
        /// row doesn't block the rest. Returns the components whose rows were saved.
        /// </summary>
        private List<TComponent> SaveComponents(List<TComponent> components)
        {
            if (components.Count == 0 || TrySaveComponents(components))
            {
                return components;
            }
            return components.Where(component => TrySaveComponents(new List<TComponent> { component })).ToList();
        }

        private bool TrySaveComponents(List<TComponent> components)
        {
            try
            {
                foreach (TComponent component in components)
                {
                    _dataContext.Update(GetModelFromComponent(component));
                }
                _dataContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save {Count} {ModelName}.", components.Count, typeof(TModel).Name);
                return false;
            }
            finally
            {
                // The components keep their model instances; a reloaded copy of a row must not clash with a tracked one.
                _dataContext.ChangeTracker.Clear();
            }
        }

        /// <summary>
        /// Deletes the components' rows, one by one when deleting them together fails. A row that is already gone
        /// counts as deleted. Returns the components whose rows are gone.
        /// </summary>
        private List<TComponent> DeleteComponents(List<TComponent> components)
        {
            if (components.Count == 0 || TryDeleteComponents(components))
            {
                return components;
            }
            return components.Where(component => TryDeleteComponents(new List<TComponent> { component })).ToList();
        }

        private bool TryDeleteComponents(List<TComponent> components)
        {
            try
            {
                foreach (TComponent component in components)
                {
                    _dataContext.Remove(GetModelFromComponent(component));
                }
                _dataContext.SaveChanges();
                return true;
            }
            catch (DbUpdateConcurrencyException) when (components.Count == 1)
            {
                // Deleted elsewhere already.
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete {Count} {ModelName}.", components.Count, typeof(TModel).Name);
                return false;
            }
            finally
            {
                _dataContext.ChangeTracker.Clear();
            }
        }

        /// <summary>
        /// Loads rows that other code or other servers inserted or changed, and removes rows they deleted.
        /// </summary>
        private void PullExternalChanges()
        {
            DateTime changesSince = _newestChangeSeen == DateTime.MinValue ? DateTime.MinValue : _newestChangeSeen - SyncOverlap;
            List<TModel> changedModels = GetModelsChangedSince(changesSince);
            foreach (TModel model in changedModels)
            {
                LoadOrReloadModel(model);
            }
            RememberNewestChange(changedModels);
            RemoveComponentsDeletedElsewhere();
        }

        private List<TModel> GetModelsChangedSince(DateTime changesSince)
        {
            return GetDbSet()
                .Where(m => m.CreatedOn > changesSince || m.UpdatedOn > changesSince)
                .AsNoTracking()
                .ToList();
        }

        private void RememberNewestChange(List<TModel> models)
        {
            foreach (TModel model in models)
            {
                DateTime newestOfModel = model.UpdatedOn > model.CreatedOn ? model.UpdatedOn : model.CreatedOn;
                if (newestOfModel > _newestChangeSeen)
                {
                    _newestChangeSeen = newestOfModel;
                }
            }
        }

        private void LoadOrReloadModel(TModel model)
        {
            EntityId entityId = GetEntityId(model);
            if (!IsEntityLoaded(entityId))
            {
                CreateEntityWithComponent(model);
                return;
            }

            // Unsaved local changes win until they are saved; up-to-date components (for example our own saves) stay.
            if (HasUnsavedChanges(entityId) || IsComponentUpToDate(entityId, model))
            {
                return;
            }
            ReloadEntity(entityId, model);
        }

        /// <summary>
        /// Destroys loaded components whose row no longer exists. A deletion elsewhere wins over local changes,
        /// because those could never be saved anymore.
        /// </summary>
        private void RemoveComponentsDeletedElsewhere()
        {
            HashSet<ulong> existingIds = GetDbSet().Select(m => m.Id).ToHashSet();
            foreach (TComponent component in GetComponents())
            {
                if (IsUnsaved(component) || existingIds.Contains(component.GetId()))
                {
                    continue;
                }

                if (component.HasChanges())
                {
                    _logger.LogWarning("{ModelName} {Id} was deleted elsewhere while it had unsaved changes here; the changes are dropped.", typeof(TModel).Name, component.GetId());
                }
                component.DestroyEntity();
            }
        }

        private bool IsEntityLoaded(EntityId entityId)
        {
            return _entityManager.Exists(entityId);
        }

        private bool HasUnsavedChanges(EntityId entityId)
        {
            TComponent? component = _entityManager.GetComponent<TComponent>(entityId);
            return component != null && (component.HasChanges() || IsMarkedDeleted(component));
        }

        /// <summary>
        /// True when the loaded component already has this version of the row, for example because this manager saved it.
        /// </summary>
        private bool IsComponentUpToDate(EntityId entityId, TModel model)
        {
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
            if (HasParentChanged(entityId, model))
            {
                _entityManager.Destroy(entityId);
                CreateEntityWithComponent(model);
                return;
            }

            _entityManager.Destroy<TComponent>(entityId);
            _entityManager.AddComponent<TComponent>(entityId, model);
        }

        private TComponent CreateEntityWithComponent(TModel model)
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
            return _entityManager.AddComponent<TComponent>(entityId, model);
        }

        /// <summary>
        /// A new component gets its database id on its first save, so it moves to the entity matching that id.
        /// </summary>
        private void RecreateEntityWithNewId(TComponent component)
        {
            TModel savedModel = GetModelFromComponent(component);
            component.ProcessChanges(false);
            component.DestroyEntity();
            CreateEntityWithComponent(savedModel);
        }
    }
}
