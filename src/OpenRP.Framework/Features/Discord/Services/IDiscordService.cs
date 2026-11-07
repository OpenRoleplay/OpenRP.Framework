using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Discord.Services
{
    public interface IDiscordService
    {
        Task SendGeneralChatMessage(string text);
        Task SendGlobalOocChatMessage(Player player, string text);
        Task SendNewbieChatMessage(Player player, string text);

        /// <summary>
        /// Queues the player count channel to show the current number of players.
        /// </summary>
        void UpdatePlayerCount(bool decrease = false);

        /// <summary>
        /// Queues the server time channel to show this in-game date and time, rounded down to the quarter hour.
        /// </summary>
        void UpdateDateTime(DateTime date, TimeSpan time);

        /// <summary>
        /// Renames the channel that has waited longest, when Discord's limit allows it. Errors are logged.
        /// </summary>
        Task SendNextChannelRenameAsync();
    }
}
