using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Shared.Chat.Enums;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Services;

namespace OpenRP.Framework.Features.RoleplayChats.Commands
{
    class LowCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Send a low whisper to nearby players. For example, /low Why are we whispering? would display \"Player says quietly: Why are we whispering?\" in chat to nearby players.",
            CommandGroups = new[] { "Chat" } )]
        public void Low(Player player, IChatService chatService, string text)
        {
            if (player.IsPlayerPlayingAsCharacter())
            {
                chatService.SendPlayerChatMessage(player, PlayerChatMessageType.LOW, text);
            }
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Short alias for /low. Example: /l [text]",
            CommandGroups = new[] { "Chat" } )]

        public void L(Player player, IChatService chatService, string text)
        {
            Low(player, chatService, text);
        }
    }
}
