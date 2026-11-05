using OpenRP.Framework.Features.Characters.Components;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Skills.Services
{
    public interface ISkillService
    {
        /// <summary>
        /// Puts the character's skills on the player, replacing the skills of any character played before.
        /// </summary>
        void LoadCharacterSkills(Player player, Character character);
    }
}
