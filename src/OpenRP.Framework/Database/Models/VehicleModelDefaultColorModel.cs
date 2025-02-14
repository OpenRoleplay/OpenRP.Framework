using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Database.Models
{
    public class VehicleModelDefaultColorModel
    {
        public ulong Id { get; set; }
        public ulong VehicleModelId { get; set; }
        public int Color1 { get; set; }
        public int Color2 { get; set; }

        // Navigational Properties
        public VehicleModelModel VehicleModel { get; set; }
    }
}
