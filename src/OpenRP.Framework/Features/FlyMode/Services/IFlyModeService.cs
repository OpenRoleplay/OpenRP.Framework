using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.FlyMode.Services
{
    public interface IFlyModeService
    {
        /// <summary>
        /// Whether the player is currently flying.
        /// </summary>
        bool IsFlying(Player player);

        /// <summary>
        /// Starts fly mode. Returns an error message, or null when the player is now flying.
        /// </summary>
        string? StartFlying(Player player);

        /// <summary>
        /// Ends fly mode and puts the player back where they were. Returns an error message, or null when they landed.
        /// </summary>
        string? StopFlying(Player player);

        /// <summary>
        /// Ends fly mode and puts the player down at the position, in the interior and virtual world, instead of where they were.
        /// Their health, armour, skin, weapons and angle are kept. Returns an error message, or null when they landed.
        /// </summary>
        string? StopFlyingAt(Player player, Vector3 position, int interior, int virtualWorld);

        /// <summary>
        /// Starts fly mode when it's off, otherwise ends it. Returns an error message, or null when it worked.
        /// </summary>
        string? ToggleFlying(Player player);

        /// <summary>
        /// The position of the object the camera is attached to while flying, otherwise null.
        /// </summary>
        Vector3? GetCameraPosition(Player player);
    }
}
