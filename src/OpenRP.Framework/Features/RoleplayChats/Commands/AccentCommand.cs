using OpenRP.Framework.Features.Characters.Components;
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
using OpenRP.Framework.Shared.Chat.Extensions;

namespace OpenRP.Framework.Features.RoleplayChats.Commands
{
    public class AccentCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Change your character's accent.",
            CommandGroups = new[] { "Character" })]
        public void Accent(Player player, string accent = "")
        {
            if (!player.IsPlayerPlayingAsCharacter())
            {
                return;
            }

            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            character.SetAccent(accent);
            SendAccentChangedMessage(player, character);
        }

        private static void SendAccentChangedMessage(Player player, Character character)
        {
            if (character.HasAccent())
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"Your accent has been set to {character.GetAccent()}.");
            }
            else
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "Your accent has been removed.");
            }
        }
    }
}
