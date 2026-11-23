using OpenRP.Framework.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Database.Models
{
    public class MapModel : BaseModel
    {
        public string Name { get; set; } = string.Empty;

        // Whether the map's objects and removed buildings are in the game. Loaded maps come back after a restart.
        public bool IsLoaded { get; set; }

        // Navigational Properties
        public List<MapObjectModel> MapObjects { get; set; } = new();
        public List<MapRemovedBuildingModel> MapRemovedBuildings { get; set; } = new();
    }
}
