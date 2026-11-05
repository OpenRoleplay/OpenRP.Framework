namespace OpenRP.Framework.Features.Skills.Helpers
{
    public static class SkillExperience
    {
        /// <summary>
        /// The total experience a skill needs to reach this level: 100 × level².
        /// </summary>
        public static uint GetExperienceForLevel(uint level)
        {
            return 100 * level * level;
        }

        public static uint GetExperienceForNextLevel(uint currentLevel)
        {
            return GetExperienceForLevel(currentLevel + 1);
        }
    }
}
