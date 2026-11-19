using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Fishing.Services;
using OpenRP.Framework.Features.Fishing.Services.Dialogs;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Fishing.Commands
{
    public class FishingNetCommands : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Fit your Fishing Net to the saved Reefer you are on or next to.",
            CommandGroups = new string[] { "Jobs", "Fishing" })]
        public void Fitnet(Player player, IFishingNetService fishingNetService)
        {
            fishingNetService.FitNet(player);
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Take the net off the Reefer you are on or next to, once its storage is empty.",
            CommandGroups = new string[] { "Jobs", "Fishing" })]
        public void Removenet(Player player, IFishingNetService fishingNetService)
        {
            fishingNetService.RemoveNet(player);
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Open the net storage of the Reefer you are on or next to.",
            CommandGroups = new string[] { "Jobs", "Fishing" })]
        public void Netstorage(Player player, IFishingNetDialogService fishingNetDialogService)
        {
            fishingNetDialogService.OpenNetStorage(player);
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Lower the net of the Reefer you are driving, or raise it early.",
            CommandGroups = new string[] { "Jobs", "Fishing" })]
        public void Trawl(Player player, ITrawlService trawlService)
        {
            trawlService.ToggleTrawl(player);
        }
    }
}
