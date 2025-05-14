using OpenRP.Framework.Features.Discord.Services;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Features.Players.Extensions;

namespace OpenRP.Framework.Features.Discord.Systems
{
    public class DiscordSystem : ISystem
    {
        [Event]
        public void OnGameModeInit(IDiscordService discordService)
        {
            discordService.UpdatePlayerCount();
        }

        [Event]
        public void OnPlayerConnect(Player player, IDiscordService discordService)
        {
            try
            {
                discordService.UpdatePlayerCount();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        [Event]
        public void OnPlayerDisconnect(Player player, DisconnectReason disconnectReason, IDiscordService discordService)
        {
            try
            {
                /*#if (!DEBUG)
                    if (player.IsPlayerPlayingAsCharacter())
                    {
                        //discordService.SendGeneralChatMessage($"## {player.Name.Replace("_", " ")} is no longer playing on the server.");
                    }
                #endif*/
                discordService.UpdatePlayerCount(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
