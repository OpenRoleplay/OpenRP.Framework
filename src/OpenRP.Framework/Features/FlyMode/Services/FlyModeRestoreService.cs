using OpenRP.Framework.Features.FlyMode.Components;
using OpenRP.Framework.Features.FlyMode.Entities;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.FlyMode.Services
{
    public class FlyModeRestoreService : IFlyModeRestoreService
    {
        public void RestoreAfterSpawn(Player player)
        {
            FlyModeRestore? restore = player.GetComponent<FlyModeRestore>();
            if (restore == null)
            {
                return;
            }

            ApplySavedState(player, restore.GetSavedState());
            restore.Destroy();
        }

        private static void ApplySavedState(Player player, FlyModeSavedState savedState)
        {
            player.Interior = savedState.Interior;
            player.VirtualWorld = savedState.VirtualWorld;
            player.Position = savedState.Position;
            player.Angle = savedState.Angle;
            player.Health = savedState.Health;
            player.Armour = savedState.Armour;
            player.Skin = savedState.Skin;
            RestoreWeapons(player, savedState.Weapons);
        }

        private static void RestoreWeapons(Player player, IEnumerable<FlyModeWeapon> weapons)
        {
            player.ResetWeapons();
            foreach (FlyModeWeapon weapon in weapons)
            {
                player.GiveWeapon(weapon.Weapon, weapon.Ammo);
            }
        }
    }
}
