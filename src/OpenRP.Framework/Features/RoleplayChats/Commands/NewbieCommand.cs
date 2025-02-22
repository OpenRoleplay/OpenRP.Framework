using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Discord.Services;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.Chat.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.RoleplayChats.Commands
{
    public class NewbieCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Send a message to the newbie chat. Use /newbie followed by your message to ask other players server-related questions.",
            CommandGroups = new[] { "Chat" })]
        public void Newbie(Player player, IChatService chatService, IDiscordService discordService, string text)
        { 
            chatService.SendPlayerChatMessage(player, PlayerChatMessageType.NEWBIE, text);
            discordService.SendNewbieChatMessage(player, text);
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Short alias for /newbie. Example: /n [text]",
            CommandGroups = new[] { "Chat" })]
        public void N(Player player, IChatService chatService, IDiscordService discordService, string text)
        {
            Newbie(player, chatService, discordService, text);
        }
    }
}
