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
    public class AmeCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Perform an action above your head. For example, /ame waves would display \"Player waves\" above your head to nearby players.",
            CommandGroups = new[] { "Chat" })]
        public void Ame(Player player, IChatService chatService, string text)
        {
            if (player.IsPlayerPlayingAsCharacter())
            {
                chatService.SendPlayerChatMessage(player, PlayerChatMessageType.AME, text);
            }
        }
    }
}
