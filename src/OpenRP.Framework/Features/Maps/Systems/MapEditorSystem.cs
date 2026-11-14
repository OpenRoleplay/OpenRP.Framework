using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Features.Maps.Services;
using OpenRP.Framework.Features.Maps.Services.Dialogs;
using OpenRP.OpenMp.Streamer;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Maps.Systems
{
    /// <summary>
    /// Clicks and moves of streamer objects. While an admin is editing a map, these go to the map editor.
    /// </summary>
    public class MapEditorSystem : ISystem
    {
        /// <summary>
        /// Returns true when the click was handled. The current streamer wrapper ignores the value, so it has no effect yet.
        /// </summary>
        [Event]
        public bool OnPlayerSelectDynamicObject(Player player, DynamicObject dynamicObject, int modelId, Vector3 position,
            IMapService mapService, IMapEditingService mapEditingService, IMapDialogService mapDialogService)
        {
            if (!mapEditingService.IsEditing(player))
            {
                return false;
            }

            mapDialogService.OnObjectClicked(player, mapService.GetMapObject(dynamicObject));
            return true;
        }

        /// <summary>
        /// Returns true when the edit of a map object was handled, while the player is editing. The current streamer wrapper ignores the value, so it has no effect yet.
        /// </summary>
        [Event]
        public bool OnPlayerEditDynamicObject(Player player, DynamicObject dynamicObject, EditObjectResponse response, Vector3 position, Vector3 rotation,
            IMapService mapService, IMapEditingService mapEditingService, IMapDialogService mapDialogService)
        {
            MapObject? mapObject = mapService.GetMapObject(dynamicObject);
            if (mapObject == null || !mapEditingService.IsEditing(player))
            {
                return false;
            }

            mapDialogService.OnObjectEdited(player, mapObject, response, position, rotation);
            return true;
        }
    }
}
