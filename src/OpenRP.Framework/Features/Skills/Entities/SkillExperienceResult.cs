namespace OpenRP.Framework.Features.Skills.Entities
{
    /// <summary>
    /// A skill's progress after experience was added, and whether that reached a new level.
    /// </summary>
    public record SkillExperienceResult(CharacterSkill Skill, bool HasLeveledUp);
}
