using OpenRP.Framework.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Database.Models
{
    public class PropertyModel : BaseModel
    {
        public string Name { get; set; }

        // Navigational Properties
        public List<PropertyDoorModel>? PropertyDoors { get; set; }
    }
}
