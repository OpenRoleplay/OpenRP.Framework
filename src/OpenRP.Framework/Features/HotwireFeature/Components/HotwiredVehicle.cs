using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.HotwireFeature.Components
{
    public class HotwiredVehicle : Component
    {
        // Indicates whether the safe hotwire method was used (Electrical Tape was present).
        public bool SafelyHotwired { get; set; }

        public int ProgressStep { get; set; } = 0;

        public Vehicle Vehicle => GetComponent<Vehicle>();

        public bool RunningEngine { get; set; }

        public void SuccessfulHotwire()
        {
            Vehicle.Engine = true;
        }

        public void TriggerHotwireElectricalShort()
        {
            Random random = new Random();
            Vehicle.Health = random.Next(250, 390);
            Vehicle.Engine = false;
            Destroy();
        }
    }
}
