using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Animations.Services
{
    public class AnimationManager : IAnimationManager
    {
        public readonly string[] _AnimLibs = new string[]
        {
            "AIRPORT", "ATTRACTORS", "BAR", "BASEBALL", "BD_FIRE",
            "BEACH", "BENCHPRESS", "BF_INJECTION", "BIKE_DBZ", "BIKED",
            "BIKEH", "BIKELEAP", "BIKES", "BIKEV", "BLOWJOBZ",
            "BMX", "BOMBER", "BOX", "BSKTBALL", "BUDDY",
            "BUS", "CAMERA", "CAR", "CAR_CHAT", "CARRY",
            "CASINO", "CHAINSAW", "CHOPPA", "CLOTHES", "COACH",
            "COLT45", "COP_AMBIENT", "COP_DVBYZ", "CRACK", "CRIB",
            "DAM_JUMP", "DANCING", "DEALER", "DILDO", "DODGE",
            "DOZER", "DRIVEBYS", "FAT", "FIGHT_B", "FIGHT_C",
            "FIGHT_D", "FIGHT_E", "FINALE", "FINALE2", "FLAME",
            "FLOWERS", "FOOD", "FREEWEIGHTS", "GANGS", "GFUNK",
            "GHANDS", "GHETTO_DB", "GOGGLES", "GRAFFITI", "GRAVEYARD",
            "GRENADE", "GYMNASIUM", "HAIRCUTS", "HEIST9", "INT_HOUSE",
            "INT_OFFICE", "INT_SHOP", "JST_BUISNESS", "KART", "KISSING",
            "KNIFE", "LAPDAN1", "LAPDAN2", "LAPDAN3", "LOWRIDER",
            "MD_CHASE", "MD_END", "MEDIC", "MISC", "MTB",
            "MUSCULAR", "NEVADA", "ON_LOOKERS", "OTB", "PARACHUTE",
            "PARK", "PAULNMAC", "PED", "PLAYER_DVBYS", "PLAYIDLES",
            "POLICE", "POOL", "POOR", "PYTHON", "QUAD",
            "QUAD_DBZ", "RAPPING", "RIFLE", "RIOT", "ROB_BANK",
            "ROCKET", "RUNNINGMAN", "RUSTLER", "RYDER", "SCRATCHING",
            "SEX", "SHAMAL", "SHOP", "SHOTGUN", "SILENCED",
            "SKATE", "SMOKING", "SNIPER", "SNM", "SPRAYCAN",
            "STRIP", "SUNBATHE", "SWAT", "SWEET", "SWIM",
            "SWORD", "TANK", "TATTOOS", "TEC", "TRAIN",
            "TRUCK", "UZI", "VAN", "VENDING", "VORTEX",
            "WAYFARER", "WEAPONS", "WOP", "WUZI"
        };

        /// <summary>
        /// Preloads all animation libraries by applying a "null" animation to the player.
        /// </summary>
        /// <param name="player">The player to apply animations to.</param>
        public void PreloadAnimLibs(Player player)
        {
            foreach (var animLib in _AnimLibs)
            {
                try
                {
                    // Apply the "null" animation from each library
                    // Parameters:
                    // animLib: The animation library name
                    // "null": The animation name
                    // 4.0f: The animation speed (delta)
                    // false: Loop the animation
                    // false: Lock X-axis
                    // false: Lock Y-axis
                    // false: Freeze the player
                    // false: Freeze the animation
                    // 0: Time (duration)
                    // 1.0f: Blend the animation
                    player.ApplyAnimation(animLib, "null", 4.0f, false, false, false, false, 0, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AnimationHelper] Failed to preload animation '{animLib}': {ex.Message}");
                }
            }
        }
    }
}
