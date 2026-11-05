using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Skills.Components;
using OpenRP.Framework.Features.Skills.Entities;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Skills.Services
{
    public class SkillService : ISkillService
    {
        private readonly BaseDataContext _dataContext;

        public SkillService(BaseDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void LoadCharacterSkills(Player player, Character character)
        {
            player.DestroyComponents<CharacterSkills>();
            player.AddComponent<CharacterSkills>(GetCharacterSkills(character.GetId()));
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
