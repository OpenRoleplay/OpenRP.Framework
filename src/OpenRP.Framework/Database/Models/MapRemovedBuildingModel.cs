using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// A GTA building removed for the players of a map, within the radius around the position.
    /// </summary>
    public class MapRemovedBuildingModel : BaseModel
    {
        public ulong MapId { get; set; }
        public int ModelId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Radius { get; set; }

        // Navigational Properties
        public MapModel? Map { get; set; }
    }
}
