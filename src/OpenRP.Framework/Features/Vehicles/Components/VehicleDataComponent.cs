using OpenRP.Framework.Database.Models;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Vehicles.Components
{
    public class VehicleDataComponent : Component
    {
        public VehicleModel VehicleData { get; set; }
    }
}
