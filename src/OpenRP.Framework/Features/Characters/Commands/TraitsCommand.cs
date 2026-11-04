using OpenRP.Framework.Features.Characters.Services.Dialogs;
using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Players.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Characters.Commands
{
    public class TraitsCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "View the traits of your character.",
            CommandGroups = new[] { "Character" })]
        public void Traits(Player player, ICharacterDialogService characterDialogService)
        {
            if (!player.IsPlayerPlayingAsCharacter())
            {
                return;
            }

            characterDialogService.OpenCharacterTraits(player);
        }
    }
}
