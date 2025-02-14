using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Characters.Components;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Characters.Services
{
    public class TempCharacterService : ITempCharacterService
    {
        private BaseDataContext _dataContext;
        public TempCharacterService(BaseDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public Character ReloadCharacter(Player player, ulong characterId)
        {
            player.DestroyComponents<Character>();

            CharacterModel? characterModel = _dataContext.Characters.FirstOrDefault(i => i.Id == characterId);

            if(characterModel != null)
            {
                return player.AddComponent<Character>(characterModel);
            }

            return null;
        }
    }
}
