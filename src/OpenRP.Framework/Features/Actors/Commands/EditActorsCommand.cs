using OpenRP.Framework.Features.Actors.Services.Dialogs;
using OpenRP.Framework.Features.Commands.Attributes;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Actors.Commands
{
    public class EditActorsCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Create actors, and move and change them: name, skin, animation and which property they're in.",
            CommandGroups = new string[] { "Admin", "Actors" })]
        public void EditActors(Player player, IActorDialogService actorDialogService)
        {
            actorDialogService.OpenActorEditor(player);
        }
    }
}
