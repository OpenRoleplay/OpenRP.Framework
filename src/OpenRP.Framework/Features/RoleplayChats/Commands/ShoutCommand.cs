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
    public class ShoutCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Send a shout to nearby players. For example, /shout Why are we shouting? would display \"Player shouts: Why are we shouting?\" in chat to nearby players.",
            CommandGroups = new[] { "Chat" } )]
        public void Shout(Player player, IChatService chatService, string text)
        {
            if (player.IsPlayerPlayingAsCharacter())
            {
                chatService.SendPlayerChatMessage(player, PlayerChatMessageType.SHOUT, text);
            }
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Short alias for /shout. Example: /s [text]",
            CommandGroups = new[] { "Chat" })]
        public void S(Player player, IChatService chatService, string text)
        {
            Shout(player, chatService, text);
        }
    }
}
