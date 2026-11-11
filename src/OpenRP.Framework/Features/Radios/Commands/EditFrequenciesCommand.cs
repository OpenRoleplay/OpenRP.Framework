using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Radios.Services.Dialogs;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Radios.Commands
{
    public class EditFrequenciesCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Create protected and encrypted radio frequencies and choose the factions on them.",
            CommandGroups = new string[] { "Admin", "Radio" })]
        public void EditFrequencies(Player player, IRadioDialogService radioDialogService)
        {
            radioDialogService.OpenFrequencyEditor(player);
        }
    }
}
