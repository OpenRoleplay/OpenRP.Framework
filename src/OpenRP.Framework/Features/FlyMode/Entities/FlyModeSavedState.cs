using SampSharp.Entities.SAMP;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.FlyMode.Entities
{
    /// <summary>
    /// Where the player was, and what they had, before they started flying. Put back when they land.
    /// </summary>
    public record FlyModeSavedState(Vector3 Position, float Angle, int Interior, int VirtualWorld, float Health, float Armour, int Skin, IReadOnlyList<FlyModeWeapon> Weapons);

    /// <summary>
    /// A weapon the player held, with its ammunition.
    /// </summary>
    public record FlyModeWeapon(Weapon Weapon, int Ammo);
}
