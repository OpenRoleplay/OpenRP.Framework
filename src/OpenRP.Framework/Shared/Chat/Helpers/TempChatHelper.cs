using OpenRP.Framework.Shared.Chat.Constants;
using OpenRP.Framework.Shared.Chat.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.Chat.Helpers
{
    public static class TempChatHelper
    {
        public static string ReturnPlayerInfoMessage(PlayerInfoMessageType type, string text)
        {
            string message = String.Empty;

            switch (type)
            {
                case PlayerInfoMessageType.INFO:
                    message = String.Format("{0}{1}", PlayerInfoMessagePrefix.INFO, text);
                    break;
                case PlayerInfoMessageType.ERROR:
                    message = String.Format("{0}{1}", PlayerInfoMessagePrefix.ERROR, text);
                    break;
                case PlayerInfoMessageType.SYNTAX:
                    message = String.Format("{0}{1}", PlayerInfoMessagePrefix.SYNTAX, text);
                    break;
                case PlayerInfoMessageType.DEBUG:
                    message = String.Format("{0}{1}", PlayerInfoMessagePrefix.DEBUG, text);
                    break;
                case PlayerInfoMessageType.ADMIN:
                    message = String.Format("{0}{1}", PlayerInfoMessagePrefix.ADMIN, text);
                    break;
            }

            return message;
        }
    }
}
