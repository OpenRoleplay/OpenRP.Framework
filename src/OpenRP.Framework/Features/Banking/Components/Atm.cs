using OpenRP.Framework.Features.Maps.Components;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Banking.Components
{
    /// <summary>
    /// An ATM: lives on the streamer object of a placed ATM map object, so it's gone when the object despawns. Its
    /// position is read from the map object, so editor moves need nothing.
    /// </summary>
    public class Atm : Component
    {
        private readonly EntityId _mapObjectEntity;

        public Atm(EntityId mapObjectEntity)
        {
            _mapObjectEntity = mapObjectEntity;
        }

        /// <summary>
        /// The map object this ATM is, or null when it's gone.
        /// </summary>
        public MapObject? GetMapObject()
        {
            return Manager.Exists(_mapObjectEntity) ? Manager.GetComponent<MapObject>(_mapObjectEntity) : null;
        }
    }
}
