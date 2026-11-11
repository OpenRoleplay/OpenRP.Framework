using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Radios.Components;
using OpenRP.Framework.Features.Radios.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Radios.Commands
{
    public class FrequencyCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Tune your radio, or the one in your vehicle, for example /freq 1500.",
            CommandGroups = new string[] { "Radio" })]
        public void Freq(Player player, IRadioService radioService, IRadioEditorService radioEditorService, IRadioFrequencyService radioFrequencyService, string frequency)
        {
            Radio? radio = radioService.GetRadioToUse(player);
            if (radio == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You don't have a radio, and the vehicle you're in has none.");
                return;
            }

            string? error = radioEditorService.SetFrequency(radio, frequency);
            if (error != null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
                return;
            }
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"Your radio is tuned to {radioFrequencyService.GetLabel(radio.GetFrequency())}.");
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Tune your radio, or the one in your vehicle, for example /freq 1500.",
            CommandGroups = new string[] { "Radio" })]
        public void Freq(Player player)
        {
            player.SendPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, "/freq [frequency]");
        }
    }
}
