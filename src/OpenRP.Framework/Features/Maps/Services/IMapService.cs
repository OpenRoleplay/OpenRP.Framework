using OpenRP.Framework.Features.Maps.Components;
using OpenRP.OpenMp.Streamer;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Maps.Services
{
    /// <summary>
    /// Reading maps and spawning their objects in the game. Changes to the data go through <see cref="IMapEditorService"/>.
    /// </summary>
    public interface IMapService
    {
        /// <summary>
        /// Every map that isn't deleted, by name. Whether a map is in the game is its IsLoaded() flag.
        /// </summary>
        IReadOnlyList<Map> GetMaps();

        /// <summary>
        /// The map with this id, or null when it doesn't exist or is deleted.
        /// </summary>
        Map? GetMap(ulong id);

        /// <summary>
        /// The map with this name, ignoring case, or null when there is none.
        /// </summary>
        Map? FindMapByName(string name);

        /// <summary>
        /// The map with this name, ignoring case, that is marked deleted but not saved yet, or null when there is none.
        /// Its name is still taken in the database until the next save.
        /// </summary>
        Map? FindDeletedMapByName(string name);

        /// <summary>
        /// Spawns the objects of every map whose IsLoaded() flag is set. Called once at startup, after the data is loaded.
        /// Until then, rows loaded or changed from the database are left for this spawn.
        /// </summary>
        void SpawnLoadedMaps();

        /// <summary>
        /// Spawns every object of the map as a streamer object, replacing any that are spawned already.
        /// </summary>
        void SpawnMap(Map map);

        /// <summary>
        /// Destroys every streamer object of the map. The data stays.
        /// </summary>
        void DespawnMap(Map map);

        /// <summary>
        /// Destroys the object's streamer object and creates it again, when its map is loaded. Used after any change
        /// that shows in the game.
        /// </summary>
        void RespawnObject(MapObject mapObject);

        /// <summary>
        /// Moves the object's streamer object to the object's position and rotation without destroying it, so it works
        /// while the object is in the streamer's edit mode. Does nothing when the object isn't spawned.
        /// </summary>
        void MoveSpawnedObject(MapObject mapObject);

        /// <summary>
        /// Destroys the object's streamer object, if it has one.
        /// </summary>
        void DespawnObject(MapObject mapObject);

        /// <summary>
        /// Respawns an object that was loaded or changed from the database, when its map is loaded, so the game shows it.
        /// Does nothing until <see cref="SpawnLoadedMaps"/> has run.
        /// </summary>
        void RespawnChangedObject(MapObject mapObject);

        /// <summary>
        /// Spawns or despawns a map that was loaded or changed from the database, to match its IsLoaded() flag. A map that
        /// gets spawned also applies its removed buildings to every online player.
        /// Does nothing until <see cref="SpawnLoadedMaps"/> has run.
        /// </summary>
        void SyncChangedMap(Map map);

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
        /// Removes a building that was loaded or changed from the database for every online player, when its map is loaded.
        /// Does nothing until <see cref="SpawnLoadedMaps"/> has run.
        /// </summary>
        void ApplyChangedRemovedBuilding(MapRemovedBuilding removal);
    }
}
