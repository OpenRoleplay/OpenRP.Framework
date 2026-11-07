using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Discord.Entities
{
    public class DiscordOptions
    {
        public string DiscordBotToken { get; set; }
        public ulong GeneralChatChannelId { get; set; }
        public ulong SupportChannelId { get; set; }
        public ulong PlayerCountChannelId { get; set; }
        /// <summary>
        /// The channel whose name shows the in-game date and time.
        /// </summary>
        public ulong ServerDateTimeChannelId { get; set; }
    }
}
