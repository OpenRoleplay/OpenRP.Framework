using OpenRP.Framework.Features.DebugSettingsFeature.Components;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Features.DebugSettingsFeature.Enums;
using SampSharp.Entities.SAMP;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.Chat.Enums;

namespace OpenRP.Framework.Features.DebugSettingsFeature.Services
{
    public sealed class DebugSettingsService : IDebugSettingsService
    {
        private readonly IDialogService _dialogService;

        public DebugSettingsService(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public DebugSettings ReloadDebugSettings(Player player)
        {
            player.DestroyComponents<DebugSettings>();

            var settings = player.AddComponent<DebugSettings>();
            return settings;
        }

        public DebugSettings GetDebugSettings(Player player)
        {
            return player.GetComponent<DebugSettings>();
        }
        
        public void ToggleDebugMessage(Player player, DebugSettings debugSettings, DebugMessageType messageType)
        {
            switch (messageType)
            {
                case DebugMessageType.Temperature:
                    debugSettings.ShowTemperatureDebugMessages = !debugSettings.ShowTemperatureDebugMessages;
                    player.SendPlayerInfoMessage(PlayerInfoMessageType.ADMIN, $"Temperature debug messages are now {GetStatusText(debugSettings.ShowTemperatureDebugMessages)}.");
                    break;
                case DebugMessageType.Time:
                    debugSettings.ShowTimeDebugMessages = !debugSettings.ShowTimeDebugMessages;
                    player.SendPlayerInfoMessage(PlayerInfoMessageType.ADMIN, $"Time debug messages are now {GetStatusText(debugSettings.ShowTimeDebugMessages)}.");
                    break;
                case DebugMessageType.Weather:
                    debugSettings.ShowWeatherDebugMessages = !debugSettings.ShowWeatherDebugMessages;
                    player.SendPlayerInfoMessage(PlayerInfoMessageType.ADMIN, $"Weather debug messages are now {GetStatusText(debugSettings.ShowWeatherDebugMessages)}.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
            }
        }

        private static string GetStatusText(bool isEnabled)
        {
            return isEnabled ? $"{Color.Green}Enabled" : $"{Color.DarkRed}Disabled";
        }

        public void OpenDebugSettingsDialog(Player player)
        {
            DebugSettings debugSettings = player.GetComponent<DebugSettings>();
            BetterTablistDialog tablistDialog = new BetterTablistDialog("Change", "Exit", 2);
            
            tablistDialog.SetTitle(TitleType.Parents, "Debug Settings");
            tablistDialog.AddHeaders("Debug Setting", "Value");

            int tempMessage = tablistDialog.AddRow("Temperature Debug Message", GetStatusText(debugSettings.ShowTemperatureDebugMessages));
            int timeMessage = tablistDialog.AddRow("Time Debug Message", GetStatusText(debugSettings.ShowTimeDebugMessages));
            int weatherMessage = tablistDialog.AddRow("Weather Debug Message", GetStatusText(debugSettings.ShowWeatherDebugMessages));


            void DialogHandler(TablistDialogResponse r)
            {
                if (r.Response != DialogResponse.LeftButton)
                {
                    return;
                }

                int index = r.ItemIndex;

                if (index == tempMessage)
                {
                    ToggleDebugMessage(player, debugSettings, DebugMessageType.Temperature);
                    OpenDebugSettingsDialog(player);
                }

                if (index == timeMessage)
                {
                    ToggleDebugMessage(player, debugSettings, DebugMessageType.Time);
                    OpenDebugSettingsDialog(player);
                }

                if (index == weatherMessage)
                {
                    ToggleDebugMessage(player, debugSettings, DebugMessageType.Weather);
                    OpenDebugSettingsDialog(player);
                }   
            }

            _dialogService.Show(player, tablistDialog, DialogHandler);
        }
    }
}
