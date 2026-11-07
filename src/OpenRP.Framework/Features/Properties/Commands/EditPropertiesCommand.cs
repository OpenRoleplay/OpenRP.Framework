using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Properties.Services.Dialogs;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Properties.Commands
{
    public class EditPropertiesCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Create properties, add and link their doors, and manage them.",
            CommandGroups = new string[] { "Admin", "Properties" })]
        public void EditProperties(Player player, IPropertyDialogService propertyDialogService)
        {
            propertyDialogService.OpenPropertyEditor(player);
        }
    }
}
