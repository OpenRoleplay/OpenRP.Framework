using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Maps.Components
{
    /// <summary>
    /// A named set of map objects and removed buildings. Its objects and removals are child entities.
    /// </summary>
    public class Map : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private readonly MapModel _cachedMapModel;
        private bool _hasChanges;
        private bool _isDeleted;

        public Map(MapModel mapModel)
        {
            _cachedMapModel = mapModel;
        }

        public bool HasChanges()
        {
            return _hasChanges;
        }

        public void ProcessChanges(bool processChanges = true)
        {
            _hasChanges = processChanges;
        }

        public bool IsDeleted()
        {
            return _isDeleted;
        }

        public void ProcessDeletion(bool processDeletion = true)
        {
            _isDeleted = processDeletion;
        }

        public MapModel GetRawMapModel()
        {
            return _cachedMapModel;
        }

        public ulong GetId()
        {
            return _cachedMapModel.Id;
        }

        public string GetName()
        {
            return _cachedMapModel.Name;
        }

        public void SetName(string name)
        {
            _cachedMapModel.Name = name;
            ProcessChanges();
        }

        /// <summary>
        /// True when the map's objects and removed buildings are in the game.
        /// </summary>
        public bool IsLoaded()
        {
            return _cachedMapModel.IsLoaded;
        }

        public void SetIsLoaded(bool isLoaded)
        {
            _cachedMapModel.IsLoaded = isLoaded;
            ProcessChanges();
        }

        /// <summary>
        /// The objects of this map that aren't deleted.
        /// </summary>
        public List<MapObject> GetObjects()
        {
            return GetComponentsInChildren<MapObject>().Where(mapObject => !mapObject.IsDeleted()).ToList();
        }

        /// <summary>
        /// The removed buildings of this map that aren't deleted.
        /// </summary>
        public List<MapRemovedBuilding> GetRemovedBuildings()
        {
            return GetComponentsInChildren<MapRemovedBuilding>().Where(removal => !removal.IsDeleted()).ToList();
        }
    }
}
