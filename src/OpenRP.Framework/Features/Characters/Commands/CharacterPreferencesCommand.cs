using OpenRP.Framework.Features.Characters.Services.Dialogs;
using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Players.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Characters.Commands
{
    public class CharacterPreferencesCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "View and change your character's preferences, such as Hardcore Mode and consent settings.",
            CommandGroups = new[] { "Character" })]
        public void CharacterPreferences(Player player, ICharacterDialogService characterDialogService)
        {
            if (!player.IsPlayerPlayingAsCharacter())
            {
                return;
            }

            characterDialogService.OpenCharacterPreferences(player);
        }
    }
}
