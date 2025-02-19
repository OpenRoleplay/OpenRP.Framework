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
        Task<bool> UpdatePlayerCount(bool decrease = false);
    }
}
