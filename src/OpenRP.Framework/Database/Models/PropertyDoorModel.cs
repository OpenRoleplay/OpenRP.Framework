using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Database.Models
{
    public class PropertyDoorModel
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public ulong? PropertyId { get; set; }
        public ulong? LinkedToPropertyDoorId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Angle { get; set; }
        public int? Interior { get; set; }

        // Navigational Properties
        public PropertyModel? Property { get; set; }
        public PropertyDoorModel? LinkedToPropertyDoor { get; set; }
    }
}
