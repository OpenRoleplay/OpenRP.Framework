using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.WorldWeather.Entities
{
    public class WorldWeatherOptions
    {
        // Weather IDs for each climate.
        public List<int> TemperateWeatherIds { get; set; } = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 13, 14, 15, 16 };
        public List<int> HumidWeatherIds { get; set; } = new List<int> { 5, 6, 7, 8, 9, 13, 14, 15, 16 };
        public List<int> AridWeatherIds { get; set; } = new List<int> { 10, 11, 12, 17, 18, 19 };

        // Min/Max temperatures for each climate (in Celsius).
        public double MinTemperatureHumid { get; set; } = 0;
        public double MaxTemperatureHumid { get; set; } = 20;

        public double MinTemperatureArid { get; set; } = 25;
        public double MaxTemperatureArid { get; set; } = 40;

        public double MinTemperatureTemperate { get; set; } = 10;
        public double MaxTemperatureTemperate { get; set; } = 25;

        // For position-based weather, define the map boundaries.
        public double MapXMin { get; set; } = -3000;
        public double MapXMax { get; set; } = 3000;
        public double MapYMin { get; set; } = -3000;
        public double MapYMax { get; set; } = 3000;

        // Seed for noise
        public int Seed { get; set; } = Environment.TickCount;
    }
}
