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

        /// <summary>
        /// The character's level in the skill with this name, or 0 when they haven't learned it yet.
        /// </summary>
        public uint GetSkillLevel(string skillName)
        {
            return _skills.FirstOrDefault(skill => skill.Name == skillName)?.Level ?? 0;
        }

        /// <summary>
        /// Puts the new progress of a skill in place of the old one, or adds the skill when it's new.
        /// </summary>
        public void SetSkill(CharacterSkill skill)
        {
            _skills.RemoveAll(existingSkill => existingSkill.SkillId == skill.SkillId);
            _skills.Add(skill);
            _skills.Sort((first, second) => string.Compare(first.Name, second.Name, System.StringComparison.Ordinal));
        }

        public bool HasSkills()
        {
            return _skills.Count > 0;
        }
    }
}
