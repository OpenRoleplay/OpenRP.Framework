using OpenRP.Framework.Features.Maps.Components;
using OpenRP.OpenMp.Streamer;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Maps.Services
{
    /// <summary>
    /// Spawning map objects as streamer objects, and removing buildings for players. The data itself lives in
    /// <see cref="IMapStore"/>, and changes to it go through <see cref="IMapEditorService"/>.
    /// </summary>
    public interface IMapService
    {
        /// <summary>
        /// Every map, ordered by name. Whether a map is in the game is its IsLoaded() flag.
        /// </summary>
        IReadOnlyList<Map> GetMaps();

        /// <summary>
        /// The map with this id, or null when there is none.
        /// </summary>
        Map? GetMap(ulong id);

        /// <summary>
        /// The map with this name, ignoring case, or null when there is none.
        /// </summary>
        Map? FindMapByName(string name);

        /// <summary>
        /// Spawns the objects of every map whose IsLoaded() flag is set. Called once at startup, after the maps are loaded.
        /// </summary>
        void SpawnLoadedMaps();

        /// <summary>
        /// Spawns every object of the map as a streamer object, replacing any that are spawned already. Runs in one go.
        /// </summary>
        void SpawnMap(Map map);

        /// <summary>
        /// Spawns every object of the map as a streamer object, a slice at a time, so the main thread keeps running.
        /// </summary>
        Task SpawnMapAsync(Map map, IProgress<int>? progress = null);

        /// <summary>
        /// Destroys every streamer object of the map. The data stays. Runs in one go.
        /// </summary>
        void DespawnMap(Map map);

        /// <summary>
        /// Destroys every streamer object of the map, a slice at a time, so the main thread keeps running.
        /// </summary>
        Task DespawnMapAsync(Map map);

        /// <summary>
        /// Destroys the object's streamer object and creates it again, when its map is loaded. Used after any change
        /// that shows in the game.
        /// </summary>
        void RespawnObject(MapObject mapObject);

        /// <summary>
        /// Moves the object's streamer object to the object's position and rotation without destroying it, so it works
        /// while the object is in the streamer's edit mode, and tells the spawn handlers of its model. Does nothing when
        /// the object isn't spawned.
        /// </summary>
        void MoveSpawnedObject(MapObject mapObject);

        /// <summary>
        /// Destroys the object's streamer object, if it has one.
        /// </summary>
        void DespawnObject(MapObject mapObject);

        /// <summary>
        /// The streamer object of the map object, or null when it isn't spawned.
        /// </summary>
        DynamicObject? GetDynamicObject(MapObject mapObject);

        /// <summary>
        /// The map object a streamer object was spawned for, or null when there is none.
        /// </summary>
        MapObject? GetMapObject(DynamicObject dynamicObject);

        /// <summary>
        /// Removes the buildings of every map that is in the game (IsLoaded() is set) for this player.
        /// </summary>
        void ApplyRemovedBuildings(Player player);

        /// <summary>
        /// Removes the building for every online player.
        /// </summary>
        void ApplyRemovedBuilding(MapRemovedBuilding removal);

        /// <summary>
        /// Removes all of this map's buildings for every online player, when the map comes into the game.
        /// </summary>
        void ApplyRemovedBuildings(Map map);
    }
}
