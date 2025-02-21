using OpenRP.Framework.Features.Commands.Attributes;
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
        private readonly ConcurrentDictionary<int, DateTime> _lastNewbieMessage = new ConcurrentDictionary<int, DateTime>();

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Send a message to the newbie chat. Use /newbie followed by your message to ask other players server-related questions.",
            CommandGroups = new[] { "Chat" })]
        public void Newbie(Player player, IChatService chatService, string text)
        { 
            if (player.IsPlayerPlayingAsCharacter())
            {
                var now = DateTime.UtcNow;

                if (_lastNewbieMessage.TryGetValue(player.Entity.Handle, out DateTime lastUsed))
                {
                    var elapsed = now - lastUsed;
                    if (elapsed.TotalSeconds < 30)
                    {
                        player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, $"You must wait {30 - elapsed.TotalSeconds:0} seconds before sending another newbie chat message.");
                        return;
                    }
                }
                _lastNewbieMessage.AddOrUpdate(player.Entity.Handle, now, (id, old) => now);

                chatService.SendPlayerChatMessage(player, PlayerChatMessageType.NEWBIE, text);
            }
        }
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Short alias for /newbie. Example: /n [text]",
            CommandGroups = new[] { "Chat" })]
        public void N(Player player, IChatService chatService, string text)
        {
            Newbie(player, chatService, text);
        }

        [Event]
        public void OnPlayerDisconnect(Player player)
        {
            _lastNewbieMessage.TryRemove(player.Entity.Handle, out _);
        }
    }
}
