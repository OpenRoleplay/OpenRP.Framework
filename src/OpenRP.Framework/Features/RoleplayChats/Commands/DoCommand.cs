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
    public class DoCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Describe the environment or context in the game world. For example, /do The sun sets over the hills would display \"The sun sets over the hills (( Player ))\" to nearby players.",
            CommandGroups = new[] { "Chat" })]
        public void Do(Player player, IChatService chatService, string text)
        {
            if (player.IsPlayerPlayingAsCharacter())
            {
                chatService.SendPlayerChatMessage(player, PlayerChatMessageType.DO, text);
            }
        }
    }
}
