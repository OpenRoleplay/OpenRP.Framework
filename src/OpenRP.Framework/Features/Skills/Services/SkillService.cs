using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Features.Skills.Components;
using OpenRP.Framework.Features.Skills.Entities;
using OpenRP.Framework.Features.Skills.Helpers;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Skills.Services
{
    public class SkillService : ISkillService
    {
        private readonly BaseDataContext _dataContext;
        private readonly ILogger<SkillService> _logger;

        public SkillService(BaseDataContext dataContext, ILogger<SkillService> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        public void LoadCharacterSkills(Player player, Character character)
        {
            player.DestroyComponents<CharacterSkills>();
            player.AddComponent<CharacterSkills>(GetCharacterSkills(character.GetId()));
        }

        public SkillExperienceResult? AddExperience(Player player, string skillName, uint experience)
        {
            Character? character = player.GetPlayerCurrentlyPlayingAsCharacter();
            SkillModel? skill = _dataContext.Skills.AsNoTracking().FirstOrDefault(skillModel => skillModel.Name == skillName);
            if (character == null || skill == null)
            {
                _logger.LogWarning("Could not add experience in skill {SkillName}: the skill or the character doesn't exist.", skillName);
                return null;
            }

            CharacterSkillModel characterSkill = GetOrAddCharacterSkillRow(character.GetId(), skill.Id);
            uint levelBefore = characterSkill.Level;
            characterSkill.Experience = SkillExperience.GetExperienceAfterGain(characterSkill.Experience, experience);
            characterSkill.Level = SkillExperience.GetLevelForExperience(characterSkill.Experience);
            characterSkill.LastUsedDate = DateTime.UtcNow;
            _dataContext.SaveChanges();

            CharacterSkill updatedSkill = new CharacterSkill(skill.Id, skill.Name, skill.Description, characterSkill.Level, characterSkill.Experience, characterSkill.LastUsedDate);
            player.GetComponent<CharacterSkills>()?.SetSkill(updatedSkill);
            return new SkillExperienceResult(updatedSkill, characterSkill.Level > levelBefore);
        }

        private CharacterSkillModel GetOrAddCharacterSkillRow(ulong characterId, ulong skillId)
        {
            CharacterSkillModel? characterSkill = _dataContext.CharacterSkills
                .FirstOrDefault(row => row.CharacterId == characterId && row.SkillId == skillId);
            if (characterSkill != null)
            {
                return characterSkill;
            }

            characterSkill = new CharacterSkillModel { CharacterId = characterId, SkillId = skillId };
            _dataContext.CharacterSkills.Add(characterSkill);
            return characterSkill;
        }

        private List<CharacterSkill> GetCharacterSkills(ulong characterId)
        {
            return _dataContext.CharacterSkills
                .Include(characterSkill => characterSkill.Skill)
                .Where(characterSkill => characterSkill.CharacterId == characterId && characterSkill.Skill != null)
                .AsNoTracking()
                .ToList()
                .Select(ToCharacterSkill)
                .OrderBy(characterSkill => characterSkill.Name)
                .ToList();
        }

        private static CharacterSkill ToCharacterSkill(CharacterSkillModel model)
        {
            return new CharacterSkill(model.SkillId, model.Skill.Name, model.Skill.Description, model.Level, model.Experience, model.LastUsedDate);
        }
    }
}
