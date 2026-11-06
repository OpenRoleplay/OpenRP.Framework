using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Shared.Commands.Services.Dialogs;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Shared.Commands.Commands
{
    public class HelpCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new[] { "Default" },
            Description = "Show a list of all commands you have access to.",
            CommandGroups = new[] { "Info" })]
        public void Help(Player player, IHelpDialogService helpDialogService)
        {
            helpDialogService.OpenHelp(player);
        }
    }
}
