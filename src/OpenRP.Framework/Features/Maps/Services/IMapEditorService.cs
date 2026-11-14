using OpenRP.Framework.Features.Maps.Components;

namespace OpenRP.Framework.Features.Maps.Services
{
    /// <summary>
    /// Creating, changing and deleting maps, their objects, materials and removed buildings, for the admin tools.
    /// Changes show in the game right away and are saved with the next map sync. Methods that can fail return the
    /// reason, or null when the change was made.
    /// </summary>
    public interface IMapEditorService
    {
        /// <summary>
        /// Creates a map, not loaded. The name is trimmed and must be 1 to 64 characters, unique ignoring case.
        /// </summary>
        string? CreateMap(string name, out Map? map);

        string? RenameMap(Map map, string name);

        /// <summary>
        /// Deletes the map with its objects, materials and removed buildings. Its objects leave the game, and every
        /// admin editing the map stops editing it.
        /// </summary>
        string? DeleteMap(Map map);

        /// <summary>
        /// Puts the map's objects in the game, and removes its buildings for every online player.
        /// </summary>
        string? LoadMap(Map map);

        /// <summary>
        /// Takes the map's objects out of the game. Its removed buildings stay removed until players reconnect.
        /// </summary>
        string? UnloadMap(Map map);

        /// <summary>
        /// Adds an object to the map. Model 0 is refused; negative models are custom models. The virtual world and
        /// interior are -1 for everywhere. The description is optional, up to 64 characters.
        /// </summary>
        string? AddObject(Map map, int modelId, Vector3 position, int virtualWorld, int interior, string? description, out MapObject? mapObject);

        string? DeleteObject(MapObject mapObject);

        /// <summary>
        /// Adds a copy of the object at the same place, with the same materials. When a material can't be copied, the
        /// copy is kept and the error says so.
        /// </summary>
        string? DuplicateObject(MapObject mapObject, out MapObject? copy);

        /// <summary>
        /// Moves the object to a position and rotation. The streamer object is moved in place, not respawned, so this
        /// is safe to call from the streamer's edit callback.
        /// </summary>
        string? MoveObject(MapObject mapObject, Vector3 position, Vector3 rotation);

        string? SetObjectModel(MapObject mapObject, int modelId);

        /// <summary>
        /// Sets the virtual world and interior of the object, -1 for everywhere.
        /// </summary>
        string? SetObjectWorld(MapObject mapObject, int virtualWorld, int interior);

        /// <summary>
        /// Sets the description, up to 64 characters. Empty clears it.
        /// </summary>
        string? SetObjectDescription(MapObject mapObject, string? description);

        /// <summary>
        /// Shows a texture on a material slot. The input is "model txd texture [ARGB colour]", and the slot is 0 to 15.
        /// Replaces whatever the slot showed.
        /// </summary>
        string? SetTexture(MapObject mapObject, int materialIndex, string input);

        /// <summary>
        /// Shows text on a material slot. The input is "text+index+font+size+bold+fontcolor+backcolor+alignment", as
        /// the old server took it. Replaces whatever the slot showed.
        /// </summary>
        string? SetMaterialText(MapObject mapObject, string input);

        /// <summary>
        /// Empties a material slot.
        /// </summary>
        string? ClearMaterial(MapObject mapObject, int materialIndex);

        /// <summary>
        /// Removes a GTA building for the map's players. The input is "model [radius]", which removes the building
        /// around the origin, or "model x y z radius". The radius defaults to 0.25. Applied to online players at once
        /// when the map is loaded.
        /// </summary>
        string? AddRemovedBuilding(Map map, string input, Vector3 origin, out MapRemovedBuilding? removal);

        /// <summary>
        /// Deletes a removed building. Players see it again after they reconnect.
        /// </summary>
        string? DeleteRemovedBuilding(MapRemovedBuilding removal);
    }
}
