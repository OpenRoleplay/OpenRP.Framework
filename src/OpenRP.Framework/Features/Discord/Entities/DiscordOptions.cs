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
    }
}
