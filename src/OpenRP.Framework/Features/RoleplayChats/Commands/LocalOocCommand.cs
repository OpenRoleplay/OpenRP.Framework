using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Discord.Services;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.RoleplayChats.Commands
{
    public class LocalOocCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Send a message to the local Out-Of-Character (OOC) chat. Use /b followed by your message to communicate OOCly with players in your vicinity.",
            CommandGroups = new[] { "Chat" } )]

        public void B(Player player, IChatService chatService, string text)
        { 
            if (player.IsPlayerPlayingAsCharacter())
            {
                chatService.SendPlayerChatMessage(player, PlayerChatMessageType.LOOC, text);
            }
        }
    }
}
