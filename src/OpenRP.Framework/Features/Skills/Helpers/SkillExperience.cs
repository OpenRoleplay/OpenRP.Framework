using System;

namespace OpenRP.Framework.Features.Skills.Helpers
{
    public static class SkillExperience
    {
        public const uint MaxLevel = 100;
        public const uint MaxExperience = ExperiencePerLevelSquared * MaxLevel * MaxLevel;

        private const uint ExperiencePerLevelSquared = 100;

        /// <summary>
        /// The total experience a skill needs to reach this level: 100 × level², up to the maximum level.
        /// </summary>
        public static uint GetExperienceForLevel(uint level)
        {
            uint cappedLevel = Math.Min(level, MaxLevel);
            return ExperiencePerLevelSquared * cappedLevel * cappedLevel;
        }

        /// <summary>
        /// The total experience for the level after this one. At the maximum level, that's the maximum experience.
        /// </summary>
        public static uint GetExperienceForNextLevel(uint currentLevel)
        {
            return IsMaxLevel(currentLevel) ? MaxExperience : GetExperienceForLevel(currentLevel + 1);
        }

        public static bool IsMaxLevel(uint level)
        {
            return level >= MaxLevel;
        }

        /// <summary>
        /// The total experience after gaining this much. Anything above the maximum isn't kept.
        /// </summary>
        public static uint GetExperienceAfterGain(uint currentExperience, uint gainedExperience)
        {
            return (uint)Math.Min((ulong)currentExperience + gainedExperience, MaxExperience);
        }

        /// <summary>
        /// The level this much total experience has reached, up to the maximum level.
        /// </summary>
        public static uint GetLevelForExperience(uint experience)
        {
            uint level = 0;
            while (!IsMaxLevel(level) && GetExperienceForNextLevel(level) <= experience)
            {
                level++;
            }
            return level;
        }
    }
}
