using Microsoft.Extensions.Options;
using OpenRP.Framework.Features.WorldWeather.Entities;
using OpenRP.Framework.Shared;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Features.WorldTime.Services;

namespace OpenRP.Framework.Features.WorldWeather.Services
{
    public class WorldWeatherService : IWorldWeatherService
    {
        private readonly IEntityManager _entityManager;
        private readonly IWorldTimeService _worldTimeService;
        private readonly WorldWeatherOptions _options;
        private readonly FastNoiseLite _cellularNoise;
        private readonly FastNoiseLite _windNoise;
        private float _windTimeOffset;

        private readonly Vector2[] _humidZonePolygon = new Vector2[]
        {
            new Vector2(-3000f, -2188f),
            new Vector2(-3000f, -1031f),
            new Vector2(-2798f, -1005f),
            new Vector2(-2556f, -867f),
            new Vector2(-2366f, -928f),
            new Vector2(-2214f, -1042f),
            new Vector2(-2103f, -1186f),
            new Vector2(-1995f, -1343f),
            new Vector2(-1889f, -1441f),
            new Vector2(-1703f, -1588f),
            new Vector2(-1597f, -1678f),
            new Vector2(-1501f, -1668f),
            new Vector2(-1388f, -1713f),
            new Vector2(-1305f, -1836f),
            new Vector2(-1227f, -1964f),
            new Vector2(-1008f, -1943f),
            new Vector2(-879f, -1910f),
            new Vector2(-800f, -1821f),
            new Vector2(-659f, -1757f),
            new Vector2(-394f, -1790f),
            new Vector2(-138f, -1824f),
            new Vector2(31f, -2215f),
            new Vector2(184f, -2709f),
            new Vector2(69f, -3000f),
            new Vector2(-1733f, -3000f),
            new Vector2(-1844f, -2498f),
            new Vector2(-1987f, -2199f),
            new Vector2(-2132f, -2001f),
            new Vector2(-2293f, -2155f),
            new Vector2(-3000f, -2188f)
        };

        private readonly Vector2[] _aridZonePolygon = new Vector2[]
        {
            new Vector2(-3000f, 3000f),
            new Vector2(-3000f, 1938f),
            new Vector2(-2308f, 1898f),
            new Vector2(-1790f, 1715f),
            new Vector2(-1325f, 1379f),
            new Vector2(-1218f, 877f),
            new Vector2(-848f, 534f),
            new Vector2(-362f, 410f),
            new Vector2(200f, 351f),
            new Vector2(644f, 537f),
            new Vector2(1002f, 628f),
            new Vector2(1115f, 633f),
            new Vector2(1459f, 556f),
            new Vector2(1895f, 497f),
            new Vector2(2180f, 485f),
            new Vector2(3000f, 463f),
            new Vector2(3000f, 3000f),
            new Vector2(-3000f, 3000f)
        };

        public WorldWeatherService(IEntityManager entityManager, IWorldTimeService worldTimeService, IOptions<WorldWeatherOptions> options)
        {
            _entityManager = entityManager;
            _worldTimeService = worldTimeService;
            _options = options.Value;

            // Cellular noise configuration
            _cellularNoise = new FastNoiseLite();
            _cellularNoise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            _cellularNoise.SetFrequency(0.0002f); 
            _cellularNoise.SetCellularJitter(0.75f); // More regular patterns
            _cellularNoise.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.EuclideanSq);
            _cellularNoise.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);
            _cellularNoise.SetSeed(_options.Seed);
            _cellularNoise.SetDomainWarpType(FastNoiseLite.DomainWarpType.BasicGrid);
            _cellularNoise.SetDomainWarpAmp(35f);

            // Configure smoother noise for wind
            _windNoise = new FastNoiseLite();
            _windNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            _windNoise.SetFrequency(0.0005f);
            _windNoise.SetSeed(_options.Seed + 1); // Different seed
            _windTimeOffset = 0f;
        }

        public int GetWeatherAt(Vector3 position)
        {
            bool isHumid = IsPositionHumid(position);
            bool isArid = IsPositionArid(position);

            // 6-hour cycles (4 per day)
            float timeFactor = (float)(DateTime.UtcNow.TimeOfDay.TotalHours / 6.0);

            float noiseValue = _cellularNoise.GetNoise(
                position.X * 0.8f,
                position.Y * 0.8f,
                timeFactor // Z-axis for time evolution
            );

            return PickWeatherForClimate(isHumid, isArid, noiseValue);
        }

        public int PredictWeather(Vector3 position, DateTime futureTime)
        {
            bool isHumid = IsPositionHumid(position);
            bool isArid = IsPositionArid(position);

            // Match the 6-hour cycle logic from GetWeatherAt
            float timeFactor = (float)(futureTime.TimeOfDay.TotalHours / 6.0);

            float noiseValue = _cellularNoise.GetNoise(
                position.X * 0.8f, // Same position scaling
                position.Y * 0.8f, // Same position scaling
                timeFactor // Future time factor on Z-axis
            );

            return PickWeatherForClimate(isHumid, isArid, noiseValue);
        }

        private int PickWeatherForClimate(bool isHumid, bool isArid, float noiseValue)
        {
            if (isHumid) return PickWeatherId(_options.HumidWeatherIds, noiseValue);
            if (isArid) return PickWeatherId(_options.AridWeatherIds, noiseValue);
            return PickWeatherId(_options.TemperateWeatherIds, noiseValue);
        }

        public double GetWindSpeedAt(Vector3 position)
        {
            // Incorporate time-based sampling for smoother changes
            _windTimeOffset += 0.1f;
            float noiseValue = _windNoise.GetNoise(
                position.X * 0.2f,
                position.Y * 0.2f,
                _windTimeOffset
            );

            // Map noise to 0-12 m/s range for realistic variation
            return (noiseValue + 1) / 2 * 12.0;
        }

        public double GetTemperatureAt(Vector3 position)
        {
            // Get climate-specific temperature range
            (double min, double max) = GetClimateTemperatureBounds(position);

            // Get current in-game time
            TimeSpan gameTime = _worldTimeService.GetCurrentSimulationTime(position.X);
            DateTime gameDate = _worldTimeService.GetCurrentIngameDateTime();

            // Calculate daily variation (-1 to 1) peaking at 3 PM
            double dailyPhase = (gameTime.TotalHours - 9) / 12; // Center at 15:00 (3 PM)
            double dailyVariation = Math.Sin(dailyPhase * Math.PI);

            // Calculate annual variation (-1 to 1) peaking in summer
            double annualPhase = (gameDate.DayOfYear / 365.0) * 2 * Math.PI;
            double annualVariation = Math.Sin(annualPhase - Math.PI / 2); // Offset for summer peak

            // Base temperature within climate range
            double baseTemp = min + (max - min) * 0.5; // Start at midpoint
            baseTemp += dailyVariation * (max - min) * 0.2; // ±20% daily swing
            baseTemp += annualVariation * (max - min) * 0.3; // ±30% annual swing

            // Elevation adjustment (1°C per 100 meters)
            baseTemp -= position.Z * 0.01;

            // Add final noise variation (±2°C)
            double noise = _windNoise.GetNoise(position.X * 0.5f, position.Y * 0.5f);
            return Math.Clamp(baseTemp + (noise * 2), min, max);
        }

        private (double min, double max) GetClimateTemperatureBounds(Vector3 position)
        {
            if (IsPositionHumid(position))
                return (_options.MinTemperatureHumid, _options.MaxTemperatureHumid);
            if (IsPositionArid(position))
                return (_options.MinTemperatureArid, _options.MaxTemperatureArid);
            return (_options.MinTemperatureTemperate, _options.MaxTemperatureTemperate);
        }

        // Helper to pick a weather id from a list based on a noise value.
        private int PickWeatherId(List<int> ids, float noiseValue)
        {
            if (ids == null || ids.Count == 0)
                throw new InvalidOperationException("No weather IDs provided for the chosen climate.");

            // Map noise from [-1, 1] to [0, 1]
            double normalized = (noiseValue + 1) / 2;
            int index = (int)(normalized * ids.Count) % ids.Count;
            return ids[index];
        }

        private bool IsPointInPolygon(Vector2 point, Vector2[] polygon)
        {
            bool inside = false;
            int j = polygon.Length - 1;
            for (int i = 0; i < polygon.Length; i++)
            {
                if ((polygon[i].Y < point.Y && polygon[j].Y >= point.Y ||
                     polygon[j].Y < point.Y && polygon[i].Y >= point.Y) &&
                    (polygon[i].X + (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < point.X))
                {
                    inside = !inside;
                }
                j = i;
            }
            return inside;
        }

        public bool IsPositionHumid(Vector3 position)
        {
            // Any x or y beyond the nominal range is considered humid.
            if(position.X < _options.MapXMin || position.X > _options.MapXMax || position.Y < _options.MapYMin || position.Y > _options.MapYMax)
            {
                return true;
            }

            return IsPointInPolygon(new Vector2(position.X, position.Y), _humidZonePolygon);
        }

        public bool IsPositionArid(Vector3 position)
        {
            return IsPointInPolygon(new Vector2(position.X, position.Y), _aridZonePolygon);
        }
    }
}
