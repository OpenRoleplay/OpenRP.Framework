using OpenRP.Framework.Features.DebugSettingsFeature.Components;
using OpenRP.Framework.Features.DebugSettingsFeature.Services;
using OpenRP.Framework.Features.WorldTime.Services;
using OpenRP.Framework.Features.WorldWeather.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.WorldWeather.Systems
{
    public class WorldWeatherSystem : ISystem
    {
        private readonly IDebugSettingsService _debugSettingsService;

        public WorldWeatherSystem(IDebugSettingsService debugSettingsService)
        {
            _debugSettingsService = debugSettingsService;
        }

        [Timer(5000)]
        public void UpdateWorldWeather(IEntityManager entityManager, IWorldWeatherService worldWeatherService)
        {
            List<Player> players = entityManager.GetComponents<Player>().ToList();
            foreach (Player player in players)
            {
                Vector3 pos = player.Position;
                int weatherId = worldWeatherService.GetWeatherAt(pos);
                player.SetWeather(weatherId);

                DebugSettings debugSettings = _debugSettingsService.GetDebugSettings(player);

                if (debugSettings != null && debugSettings.ShowWeatherDebugMessages)
                {
                    bool isHumid = worldWeatherService.IsPositionHumid(pos);
                    bool isArid = worldWeatherService.IsPositionArid(pos);
                    double windSpeed = worldWeatherService.GetWindSpeedAt(pos);
                    double temp = worldWeatherService.GetTemperatureAt(pos);

                    player.SendPlayerInfoMessage(PlayerInfoMessageType.DEBUG,
                        $"Weather: {weatherId} | " +
                        $"Humid: {(isHumid ? "Yes" : "No")} | " +
                        $"Arid: {(isArid ? "Yes" : "No")} | " +
                        $"Wind: {windSpeed:F1} m/s | " +
                        $"Temp: {temp:F1} degrees Celsius");
                }
            }
        }
    }
}
