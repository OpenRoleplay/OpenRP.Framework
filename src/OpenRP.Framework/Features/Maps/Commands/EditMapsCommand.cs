using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Maps.Services.Dialogs;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Maps.Commands
{
    public class EditMapsCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Create, load and edit maps of objects.",
            CommandGroups = new string[] { "Admin", "Maps" })]
        public void EditMaps(Player player, IMapDialogService mapDialogService)
        {
            mapDialogService.OpenMapEditor(player);
        }
    }
}
