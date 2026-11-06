using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Accounts.Components;
using OpenRP.Framework.Features.Characters.Components;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Characters.Services
{
    public interface ICharacterService
    {
        List<CharacterModel> GetCharactersByAccountId(ulong accountId);

        /// <summary>
        /// The "First Last" names of these characters, online or not, by character id.
        /// </summary>
        Dictionary<ulong, string> GetCharacterNames(IEnumerable<ulong> characterIds);
        bool CreateCharacter(Account account, CharacterModel newCharacter);
        Character? LoadCharacter(Player player, ulong characterId);
        Task SelectCharacterAsync(Player player, ulong characterId);
        void SaveCharacter(Character character);
        void SaveCharacterPreferences(CharacterPreferences characterPreferences);
        void SaveChangedCharacters();
    }
}
