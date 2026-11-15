using OpenRP.Framework.Database.Entities;
using System.Collections.Generic;

namespace OpenRP.Framework.Database.Models
{
    public class MapObjectModel : BaseModel
    {
        public ulong MapId { get; set; }

        // Negative model IDs are custom models.
        public int ModelId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float RotX { get; set; }
        public float RotY { get; set; }
        public float RotZ { get; set; }

        // -1 means everywhere: every virtual world, or every interior.
        public int VirtualWorld { get; set; } = -1;
        public int Interior { get; set; } = -1;

        // A note for the editors, so the objects of a map can be told apart.
        public string? Description { get; set; }

        // Metres. Null means the default, see MapLimits.DefaultStreamDistance.
        public float? StreamDistance { get; set; }

        // Navigational Properties
        public MapModel? Map { get; set; }
        public List<MapObjectMaterialModel> Materials { get; set; } = new();
    }
}
