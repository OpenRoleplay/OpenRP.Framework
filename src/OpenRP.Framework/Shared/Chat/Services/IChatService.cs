using OpenRP.Framework.Shared.Chat.Enums;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.Chat.Services
{
    public interface IChatService
    {
        void SendInfoMessage(PlayerInfoMessageType type, string text);
        void SendTalkMessage(string name, Vector3 position, string text, string accent = null);
        void SendTalkMessage(Player player, string text);
        void SendPlayerChatMessage(Player player, PlayerChatMessageType type, string text);
    }
}
