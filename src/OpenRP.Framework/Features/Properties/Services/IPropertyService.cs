using OpenRP.Framework.Features.Properties.Components;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Properties.Services
{
    public interface IPropertyService
    {
        /// <summary>
        /// Moves the player through the door they're standing at to the door it leads to, or tells them why not.
        /// </summary>
        void UseNearbyDoor(Player player);

        /// <summary>
        /// The door the player is standing at, or null when there is none.
        /// </summary>
        PropertyDoor? GetNearbyDoor(Player player);

        /// <summary>
        /// The loaded door with this id, or null.
        /// </summary>
        PropertyDoor? GetDoor(ulong doorId);

        /// <summary>
        /// The virtual world the door is in: 0 outside, otherwise the virtual world of its property.
        /// </summary>
        int GetDoorVirtualWorld(PropertyDoor door);

        /// <summary>
        /// Creates the text label above the door, replacing the one it had.
        /// </summary>
        void CreateDoorLabel(PropertyDoor door);
    }
}
