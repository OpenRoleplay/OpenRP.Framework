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
    public class GlobalOocCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Send a message to the global Out-Of-Character (OOC) chat. Use /ooc followed by your message to communicate OOCly with all players.",
            CommandGroups = new[] { "Chat" })]
        public void Ooc(Player player, IChatService chatService, IDiscordService discordService, string text)
        {
            if (player.IsPlayerPlayingAsCharacter())
            {
                chatService.SendPlayerChatMessage(player, PlayerChatMessageType.OOC, text);
                discordService.SendGlobalOocChatMessage(player, text);
            }
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Send a message to the global Out-Of-Character (OOC) chat. Use /o followed by your message to communicate OOCly with all players.",
            CommandGroups = new[] { "Chat" })]
        public void O(Player player, IChatService chatService, IDiscordService discordService, string text)
        {
            Ooc(player, chatService, discordService, text);
        }
    }
}
