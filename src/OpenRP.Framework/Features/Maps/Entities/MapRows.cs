using OpenRP.Framework.Database.Models;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Maps.Entities
{
    /// <summary>
    /// Every map row in the database, read in one go at startup.
    /// </summary>
    public record MapRows(
        List<MapModel> Maps,
        List<MapObjectModel> Objects,
        List<MapObjectMaterialModel> Materials,
        List<MapRemovedBuildingModel> RemovedBuildings);
}
