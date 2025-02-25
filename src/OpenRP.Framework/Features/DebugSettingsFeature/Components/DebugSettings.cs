using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.DebugSettingsFeature.Components
{
    public class DebugSettings : Component
    {
        public bool ShowTemperatureDebugMessages { get; set; }
        public bool ShowTimeDebugMessages { get; set; }
        public bool ShowWeatherDebugMessages { get; set; }
    }
}