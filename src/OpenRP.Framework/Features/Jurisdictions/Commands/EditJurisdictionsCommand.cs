using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Jurisdictions.Services.Dialogs;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Jurisdictions.Commands
{
    public class EditJurisdictionsCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Create countries and the places in them, and set their tags, currency, government and tax registrations.",
            CommandGroups = new string[] { "Admin", "Jurisdictions" })]
        public void EditJurisdictions(Player player, IJurisdictionDialogService jurisdictionDialogService)
        {
            jurisdictionDialogService.OpenJurisdictionEditor(player);
        }
    }
}
