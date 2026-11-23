using OpenRP.Framework.Database.Models;
using SampSharp.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Maps.Components
{
    /// <summary>
    /// A named set of map objects. Its removed buildings are child entities. Its objects are root entities that point
    /// back to it, see <see cref="MapObject"/>. Only the map store changes its state.
    /// </summary>
    public class Map : Component
    {
        private readonly MapModel _cachedMapModel;

        public Map(MapModel mapModel)
        {
            _cachedMapModel = mapModel;
        }

        public ulong GetId()
        {
            return _cachedMapModel.Id;
        }

        public string GetName()
        {
            return _cachedMapModel.Name;
        }

        internal void SetName(string name)
        {
            _cachedMapModel.Name = name;
        }

        /// <summary>
        /// True when the map's objects and removed buildings are in the game.
        /// </summary>
        public bool IsLoaded()
        {
            return _cachedMapModel.IsLoaded;
        }

        internal void SetIsLoaded(bool isLoaded)
        {
            _cachedMapModel.IsLoaded = isLoaded;
        }

        /// <summary>
        /// The removed buildings of this map.
        /// </summary>
        public List<MapRemovedBuilding> GetRemovedBuildings()
        {
            return GetComponentsInChildren<MapRemovedBuilding>().ToList();
        }
    }
}
