using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Models
{
    public class PropertyModel
    {
        public ulong Id { get; set; }
        public string Name { get; set; }

        // Navigational Properties
        public List<PropertyDoorModel>? PropertyDoors { get; set; }
    }
}
