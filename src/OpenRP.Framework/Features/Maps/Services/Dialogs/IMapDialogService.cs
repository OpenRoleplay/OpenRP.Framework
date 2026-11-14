using OpenRP.Framework.Features.Maps.Components;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Maps.Services.Dialogs
{
    /// <summary>
    /// The in-game map editor: the map list, the map and editor menus, and the object, material and removed building menus.
    /// </summary>
    public interface IMapDialogService
    {
        /// <summary>
        /// Opens the /editmaps menus: the editor of the map the admin is editing, or else the list of maps.
        /// </summary>
        void OpenMapEditor(Player player);

        /// <summary>
        /// Handles a click on an object in select mode. Opens the object's menu, or explains why the object can't be selected.
        /// </summary>
        void OnObjectClicked(Player player, MapObject? mapObject);

        /// <summary>
        /// Handles the end of a move or rotate. Final saves the new place, Cancel keeps the old one, and then the object's menu opens.
        /// </summary>
        void OnObjectEdited(Player player, MapObject mapObject, EditObjectResponse response, Vector3 position, Vector3 rotation);
    }
}
