using OpenRP.Framework.Features.Maps.Components;
using OpenRP.OpenMp.Streamer;

namespace OpenRP.Framework.Features.Maps.Entities
{
    /// <summary>
    /// Attaches behaviour to the map objects of some models when they spawn, for example a harvestable plant or a tree.
    /// Registered with AddMapObjectSpawnHandler. It's a direct, per-model call rather than a server event, because it runs
    /// once for every object, up to about 400k times at startup, and a server event would be published and matched each time.
    /// </summary>
    public interface IMapObjectSpawnHandler
    {
        /// <summary>
        /// True when this handler wants the map objects of the model.
        /// </summary>
        bool Handles(int modelId);

        /// <summary>
        /// Called right after the streamer object is created and its materials are set. Attach behaviour to the
        /// DynamicObject's entity, so it goes away when the object despawns.
        /// </summary>
        void OnMapObjectSpawned(MapObject mapObject, DynamicObject dynamicObject);

        /// <summary>
        /// Called after an editor move put the streamer object somewhere else without respawning it.
        /// </summary>
        void OnMapObjectMoved(MapObject mapObject, DynamicObject dynamicObject) { }
    }
}
