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
            IMapService mapService, IMapEditingService mapEditingService, IMapBusyTracker mapBusyTracker, IMapDialogService mapDialogService)
        {
            if (!mapEditingService.IsEditing(player))
            {
                return false;
            }

            MapObject? mapObject = mapService.GetMapObject(dynamicObject);
            if (IsMapBusy(mapObject, mapBusyTracker))
            {
                return false;
            }

            mapDialogService.OnObjectClicked(player, mapObject);
            return true;
        }

        /// <summary>
        /// Returns true when the edit of a map object was handled, while the player is editing. The current streamer wrapper ignores the value, so it has no effect yet.
        /// </summary>
        [Event]
        public bool OnPlayerEditDynamicObject(Player player, DynamicObject dynamicObject, EditObjectResponse response, Vector3 position, Vector3 rotation,
            IMapService mapService, IMapEditingService mapEditingService, IMapBusyTracker mapBusyTracker, IMapDialogService mapDialogService)
        {
            MapObject? mapObject = mapService.GetMapObject(dynamicObject);
            if (mapObject == null || !mapEditingService.IsEditing(player) || IsMapBusy(mapObject, mapBusyTracker))
            {
                return false;
            }

            mapDialogService.OnObjectEdited(player, mapObject, response, position, rotation);
            return true;
        }

        /// <summary>
        /// A busy map (being replaced or deleted) owns its objects, so clicks and edits of them are ignored until it's done.
        /// </summary>
        private static bool IsMapBusy(MapObject? mapObject, IMapBusyTracker mapBusyTracker)
        {
            return mapBusyTracker.GetReason(mapObject?.GetMap()) != null;
        }
    }
}
