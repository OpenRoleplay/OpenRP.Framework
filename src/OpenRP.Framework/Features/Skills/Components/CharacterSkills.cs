using OpenRP.Framework.Features.Skills.Entities;
using SampSharp.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Skills.Components
{
    /// <summary>
    /// The skills of the character the player is playing as. Loaded when the character is selected.
    /// </summary>
    public class CharacterSkills : Component
    {
        private readonly List<CharacterSkill> _skills;

        public CharacterSkills(List<CharacterSkill> skills)
        {
            _skills = skills;
        }

        public IReadOnlyList<CharacterSkill> GetSkills()
        {
            return _skills;
        }

        public CharacterSkill? GetSkill(ulong skillId)
        {
            return _skills.FirstOrDefault(skill => skill.SkillId == skillId);
        }

        public bool HasSkills()
        {
            return _skills.Count > 0;
        }
    }
}
