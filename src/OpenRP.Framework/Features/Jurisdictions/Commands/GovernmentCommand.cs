using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Jurisdictions.Services.Dialogs;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Jurisdictions.Commands
{
    public class GovernmentCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Change the settings of the jurisdictions your faction governs.",
            CommandGroups = new string[] { "Jurisdictions" })]
        public void Government(Player player, IJurisdictionDialogService jurisdictionDialogService)
        {
            jurisdictionDialogService.OpenGovernment(player);
        }
    }
}
