using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Characters.Services;
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
        public void Accent(Player player, ITempCharacterService characterService, string accent = "")
        {
            if (player.IsPlayerPlayingAsCharacter())
            {
                Character character = player.GetComponent<Character>();

                bool result = characterService.SetCharacterAccent(character, accent);

                if (result)
                {
                    player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"Your accent has been set to {accent}.");
                }
                else
                {
                    player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, $"Your accent could not be set to {accent}. Make a bug report with the following timestamp: {DateTime.Now.ToString()}.");
                }
            }
        }
    }
}
