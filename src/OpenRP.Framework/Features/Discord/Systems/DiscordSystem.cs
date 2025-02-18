using OpenRP.Framework.Features.Discord.Services;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Discord.Systems
{
    public class DiscordSystem : ISystem
    {
        [Event]
        public void OnGameModeInit(IDiscordService discordService)
        {
        }

        [Event]
        public void OnPlayerDisconnect(Player player, DisconnectReason disconnectReason, IDiscordService discordService)
        {
            try
            {
                #if (!DEBUG)
                    if (player.IsPlayerPlayingAsCharacter())
                    {
                        discordService.SendGeneralChatMessage($"## {player.Name.Replace("_", " ")} is no longer playing on the server.");
                    }
                #endif
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
