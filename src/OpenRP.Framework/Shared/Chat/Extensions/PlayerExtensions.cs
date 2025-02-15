using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Helpers;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.Chat.Extensions
{
    public static class PlayerExtensions
    {
        public static void SendPlayerInfoMessage(this Player player, PlayerInfoMessageType type, string text)
        {
            string message = TempChatHelper.ReturnPlayerInfoMessage(type, text);

            player.SendClientMessage(message);
        }
    }
}
