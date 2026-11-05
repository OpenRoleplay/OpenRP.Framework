using OpenRP.Framework.Features.Skills.Helpers;
using System;

namespace OpenRP.Framework.Features.Skills.Entities
{
    /// <summary>
    /// How far a character is in one skill.
    /// </summary>
    public record CharacterSkill(ulong SkillId, string Name, string? Description, uint Level, uint Experience, DateTime? LastUsedDate)
    {
        public uint GetExperienceForNextLevel()
        {
            return SkillExperience.GetExperienceForNextLevel(Level);
        }

        public bool HasDescription()
        {
            return !string.IsNullOrEmpty(Description);
        }
    }
}
