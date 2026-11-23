using OpenRP.Framework.Database.Models;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Maps.Components
{
    /// <summary>
    /// A GTA building removed for the players of a map. A child entity of the map. Removals are applied to players
    /// when they connect, so deleting one takes effect after they reconnect.
    /// </summary>
    public class MapRemovedBuilding : Component
    {
        private readonly MapRemovedBuildingModel _removalModel;

        public MapRemovedBuilding(MapRemovedBuildingModel removalModel)
        {
            _removalModel = removalModel;
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
    }
}
