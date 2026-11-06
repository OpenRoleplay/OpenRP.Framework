using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Properties.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Properties.Commands
{
    public class EnterCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Go through the property door you're standing at.",
            CommandGroups = new string[] { "Properties" })]
        public void Enter(Player player, IPropertyService propertyService)
        {
            propertyService.UseNearbyDoor(player);
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Go through the property door you're standing at.",
            CommandGroups = new string[] { "Properties" })]
        public void Exit(Player player, IPropertyService propertyService)
        {
            propertyService.UseNearbyDoor(player);
        }
    }
}
