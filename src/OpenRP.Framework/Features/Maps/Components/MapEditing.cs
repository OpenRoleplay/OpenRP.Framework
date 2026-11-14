using SampSharp.Entities;

namespace OpenRP.Framework.Features.Maps.Components
{
    /// <summary>
    /// The map an admin is editing, on the admin's player entity. It stays until the admin stops editing or starts
    /// editing another map. The selected object is the last object the admin picked in the editor.
    /// </summary>
    public class MapEditing : Component
    {
        private readonly ulong _mapId;
        private ulong? _selectedObjectId;

        public MapEditing(ulong mapId)
        {
            _mapId = mapId;
        }

        public ulong GetMapId()
        {
            return _mapId;
        }

        public ulong? GetSelectedObjectId()
        {
            return _selectedObjectId;
        }

        public void SetSelectedObjectId(ulong? mapObjectId)
        {
            _selectedObjectId = mapObjectId;
        }
    }
}
