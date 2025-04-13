using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Database.Services;
using OpenRP.Framework.Features.Characters.Services;
using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Inventories.Services;
using OpenRP.Framework.Features.Items.Components;
using OpenRP.Framework.Shared.Chat.Enums;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Features.Admins.Components;

namespace OpenRP.Framework.Features.Admins.Commands
{
    public class SetWeatherTimeCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Allows you to override the weather and/or time.")]
        public void SetWeatherTime(Player player, int weather, int hour, int minutes)
        {
            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();

            WeatherTimeOverride weatherTimeOverride = character.GetComponent<WeatherTimeOverride>();
            if (weatherTimeOverride == null)
            {
                weatherTimeOverride = character.AddComponent<WeatherTimeOverride>();
            }

            player.SetWeather(weather);
            player.SetTime(hour, minutes);
            player.SendPlayerInfoMessage(PlayerInfoMessageType.ADMIN, $"Weather has been set to {weather} and Time has been set to {hour}:{minutes}.");
        }
    }
}
