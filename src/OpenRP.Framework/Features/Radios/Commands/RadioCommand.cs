using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Features.Radios.Services.Dialogs;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Radios.Commands
{
    public class RadioCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Use your radio, or the one in your vehicle: power, frequency and earpiece.",
            CommandGroups = new string[] { "Radio" })]
        public void Radio(Player player, IRadioDialogService radioDialogService)
        {
            if (player.IsPlayerPlayingAsCharacter())
            {
                radioDialogService.OpenRadio(player);
            }
        }
    }
}
