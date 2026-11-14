using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Maps.Components
{
    /// <summary>
    /// A GTA building removed for the players of a map. A child entity of the map. Removals are applied to players
    /// when they connect, so deleting one takes effect after they reconnect.
    /// </summary>
    public class MapRemovedBuilding : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private readonly MapRemovedBuildingModel _removalModel;
        private bool _hasChanges;
        private bool _isDeleted;

        public MapRemovedBuilding(MapRemovedBuildingModel removalModel)
        {
            _removalModel = removalModel;
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

        public MapRemovedBuildingModel GetRawMapRemovedBuildingModel()
        {
            return _removalModel;
        }

        public ulong GetId()
        {
            return _removalModel.Id;
        }

        public int GetModelId()
        {
            return _removalModel.ModelId;
        }

        public Vector3 GetPosition()
        {
            return new Vector3(_removalModel.X, _removalModel.Y, _removalModel.Z);
        }

        public float GetRadius()
        {
            return _removalModel.Radius;
        }

        /// <summary>
        /// The map this removal belongs to, or null when it has none.
        /// </summary>
        public Map? GetMap()
        {
            return GetComponentInParent<Map>();
        }
    }
}
