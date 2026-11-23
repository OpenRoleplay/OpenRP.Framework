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
            // open.mp NPCs connect as players, but they aren't players for the count.
            if (player.IsNpc)
            {
                return;
            }
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
            if (player.IsNpc)
            {
                return;
            }
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
