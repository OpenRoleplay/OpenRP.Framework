using OpenRP.Framework.Features.Commands.Attributes;
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
    public class SayCommands : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Send a message in In-Character (IC) proximity chat. Use /say followed by your message to communicate with players nearby your character.",
            CommandGroups = new[] { "Chat" })]
        public void Say(Player player, IChatService chatService, string text)
        {
            if (player.IsPlayerPlayingAsCharacter())
            {
                chatService.SendTalkMessage(player, text);
            }
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Send a message in In-Character (IC) proximity chat. Use /talk followed by your message to communicate with players nearby your character.",
            CommandGroups = new[] { "Chat" })]
        public void Talk(Player player, IChatService chatService, string text)
        {
            Say(player, chatService, text);
        }
    }
}
