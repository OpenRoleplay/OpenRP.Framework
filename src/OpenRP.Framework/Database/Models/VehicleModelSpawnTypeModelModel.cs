using OpenRP.Framework.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Database.Models
{
    public class VehicleModelSpawnTypeModelModel : BaseModel
    {
        public ulong VehicleSpawnTypeId { get; set; }
        public ulong VehicleModelId { get; set; }

        // Navginational Properties
        public VehicleModelModel VehicleModel { get; set; }
        public VehicleModelSpawnTypeModel VehicleSpawnType { get; set; }
    }
}
