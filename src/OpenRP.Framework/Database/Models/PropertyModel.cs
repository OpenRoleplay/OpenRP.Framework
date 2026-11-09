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
        // The jurisdiction the property lies in, where it would pay property tax; null for none.
        public ulong? JurisdictionId { get; set; }

        // Navigational Properties
        public List<PropertyDoorModel>? PropertyDoors { get; set; }
        public JurisdictionModel? Jurisdiction { get; set; }
    }
}
