using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Characters.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Characters.Systems
{
    public class CharacterPersistenceSystem : ISystem
    {
        private const int SaveIntervalInMilliseconds = 60000;

        [Timer(SaveIntervalInMilliseconds)]
        public void SaveChangedCharacters(ICharacterService characterService)
        {
            characterService.SaveChangedCharacters();
        }

        [Event]
        public void OnPlayerDisconnect(Player player, DisconnectReason disconnectReason, ICharacterService characterService)
        {
            SaveCharacterIfChanged(player, characterService);
            SaveCharacterPreferencesIfChanged(player, characterService);
        }

        private static void SaveCharacterIfChanged(Player player, ICharacterService characterService)
        {
            Character? character = player.GetComponent<Character>();
            if (character != null && character.HasChanges())
            {
                characterService.SaveCharacter(character);
            }
        }

        private static void SaveCharacterPreferencesIfChanged(Player player, ICharacterService characterService)
        {
            CharacterPreferences? preferences = player.GetComponent<CharacterPreferences>();
            if (preferences != null && preferences.HasChanges())
            {
                characterService.SaveCharacterPreferences(preferences);
            }
        }
    }
}
