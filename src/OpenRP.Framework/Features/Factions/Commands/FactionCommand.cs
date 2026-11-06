using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Factions.Services.Dialogs;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Factions.Commands
{
    public class FactionCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Open the menu of your faction: members, ranks, invitations and sub-factions.",
            CommandGroups = new string[] { "Factions" })]
        public void Faction(Player player, IFactionDialogService factionDialogService)
        {
            factionDialogService.OpenFactions(player);
        }
    }
}
