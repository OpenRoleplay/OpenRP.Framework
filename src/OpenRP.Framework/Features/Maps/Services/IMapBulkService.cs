using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Features.Maps.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Maps.Services
{
    /// <summary>
    /// Replaces every object of a map at once, for generated maps such as the world. The rows are written in one
    /// transaction on a worker thread, then the objects are put in the game in slices on the main thread.
    /// </summary>
    public interface IMapBulkService
    {
        /// <summary>
        /// Replaces the map's objects, and their textures, with the given ones, and loads the map. Every object is checked
        /// first, and the database changes only when all of them pass. Editing, loading, unloading and deleting the map are
        /// refused until this returns. If the objects were saved but some couldn't be put in the game, the map stays refused until
        /// the server restarts. Progress is reported on the main thread.
        /// </summary>
        Task<MapBulkResult> ReplaceObjectsAsync(Map map, IReadOnlyList<MapObjectSpec> objects, IProgress<MapBulkProgress>? progress = null);
    }
}
