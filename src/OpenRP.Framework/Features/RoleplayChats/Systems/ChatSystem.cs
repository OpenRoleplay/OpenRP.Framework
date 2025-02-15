using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Shared.Chat.Services;

namespace OpenRP.Framework.Features.RoleplayChats.Systems
{
    public class ChatSystem : ISystem
    {
        [Event]
        public bool OnPlayerText(Player player, string text, IChatService chatService)
        {
            chatService.SendTalkMessage(player, text);

            return false;
        }
    }
}
