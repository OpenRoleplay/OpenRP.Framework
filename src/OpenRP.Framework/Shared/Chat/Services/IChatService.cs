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
        void SendTalkMessage(string name, Vector3 position, string text, string accent = null, int virtualWorld = 0);
        void SendTalkMessage(Player player, string text);

        /// <summary>
        /// Sends "* Name text *" to players near the position, like /me.
        /// </summary>
        void SendActionMessage(string name, Vector3 position, int virtualWorld, string text);

        /// <summary>
        /// Sends "text (( Name ))" to players near the position, like /do.
        /// </summary>
        void SendDoMessage(string name, Vector3 position, int virtualWorld, string text);
        void SendPlayerChatMessage(Player player, PlayerChatMessageType type, string text);
    }
}
