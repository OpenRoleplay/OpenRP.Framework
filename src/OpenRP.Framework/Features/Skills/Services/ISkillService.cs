using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Skills.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Skills.Services
{
    public interface ISkillService
    {
        /// <summary>
        /// Puts the character's skills on the player, replacing the skills of any character played before.
        /// </summary>
        void LoadCharacterSkills(Player player, Character character);

        /// <summary>
        /// Gives the player's character experience in the skill with this name, and raises its level when it's reached.
        /// Experience above the maximum level isn't kept.
        /// Returns the new progress, or null when the host didn't seed this skill.
        /// </summary>
        SkillExperienceResult? AddExperience(Player player, string skillName, uint experience);
    }
}
