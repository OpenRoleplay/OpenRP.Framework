using OpenRP.Framework.Features.Maps.Components;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Maps.Services
{
    /// <summary>
    /// The admin's editing session: the map they edit, the object they selected, and where edits start from.
    /// </summary>
    public interface IMapEditingService
    {
        /// <summary>
        /// Whether the player is editing a map. While they are, clicks and moves of objects go to the map editor.
        /// </summary>
        bool IsEditing(Player player);

        /// <summary>
        /// The map the player is editing, or null when they aren't editing one or it has been deleted.
        /// </summary>
        Map? GetEditedMap(Player player);

        /// <summary>
        /// Makes the map the one the player edits. A session on another map is replaced.
        /// </summary>
        void StartEditing(Player player, Map map);

        /// <summary>
        /// Ends the session: leaves edit mode, stops flying, and forgets the selected object.
        /// </summary>
        void StopEditing(Player player);

        /// <summary>
        /// Ends the session of every player who is editing this map, for example when it is deleted. Sessions on other maps are left alone.
        /// </summary>
        void StopEditingMap(Map map);

        /// <summary>
        /// Remembers the object as selected and leaves edit mode. Returns the reason the object can't be selected,
        /// or null when it was selected.
        /// </summary>
        string? SelectObject(Player player, MapObject? mapObject);

        /// <summary>
        /// Forgets the object when it is the selected one, for example after it was deleted.
        /// </summary>
        void ForgetObject(Player player, MapObject mapObject);

        /// <summary>
        /// The point the editor works from: the camera while flying, otherwise the player's position.
        /// </summary>
        Vector3 GetEditorOrigin(Player player);

        /// <summary>
        /// Where new objects go: in front of the camera while flying, otherwise in front of the player.
        /// </summary>
        Vector3 GetPointInFrontOfEditor(Player player);
    }
}
