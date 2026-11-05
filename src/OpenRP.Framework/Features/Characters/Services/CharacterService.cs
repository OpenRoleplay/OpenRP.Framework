using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Entities;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Accounts.Components;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Inventories.Services;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using OpenRP.Framework.Shared.ServerEvents.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Characters.Services
{
    public class CharacterService : ICharacterService
    {
        private const uint CharacterInventoryMaxWeightInGrams = 10000;

        private readonly BaseDataContext _dataContext;
        private readonly IEntityManager _entityManager;
        private readonly IInventoryManager _inventoryManager;
        private readonly IServerEventAggregator _serverEventAggregator;
        private readonly ILogger<CharacterService> _logger;

        public CharacterService(BaseDataContext dataContext, IEntityManager entityManager, IInventoryManager inventoryManager, IServerEventAggregator serverEventAggregator, ILogger<CharacterService> logger)
        {
            _dataContext = dataContext;
            _entityManager = entityManager;
            _inventoryManager = inventoryManager;
            _serverEventAggregator = serverEventAggregator;
            _logger = logger;
        }

        public List<CharacterModel> GetCharactersByAccountId(ulong accountId)
        {
            return _dataContext.Characters
                .AsNoTracking()
                .Where(character => character.AccountId == accountId)
                .ToList();
        }

        /// <summary>
        /// Saves a new character for the account. BaseDataContext gives every new character an inventory.
        /// </summary>
        public bool CreateCharacter(Account account, CharacterModel newCharacter)
        {
            try
            {
                newCharacter.AccountId = account.GetAccountId();
                SaveModel(newCharacter);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not create character {FirstName} {LastName}.", newCharacter.FirstName, newCharacter.LastName);
                return false;
            }
        }

        /// <summary>
        /// Puts the character and its preferences on the player, replacing any character the player had.
        /// </summary>
        public Character? LoadCharacter(Player player, ulong characterId)
        {
            UnloadCharacter(player);

            CharacterModel? characterModel = GetCharacterModelById(characterId);
            if (characterModel == null)
            {
                return null;
            }

            EnsureCharacterHasLoadedInventory(characterModel);

            Character character = player.AddComponent<Character>(characterModel);
            player.AddComponent<CharacterPreferences>(GetOrCreateCharacterPreferencesModel(characterId));
            return character;
        }

        /// <summary>
        /// Loads the character onto the player and announces it with the OnCharacterSelected server event.
        /// </summary>
        public async Task SelectCharacterAsync(Player player, ulong characterId)
        {
            try
            {
                Character? character = LoadCharacter(player, characterId);
                if (character == null)
                {
                    _logger.LogWarning("Character {CharacterId} could not be loaded for {PlayerName}.", characterId, player.Name);
                    return;
                }

                await PublishCharacterSelected(player, character);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Selecting character {CharacterId} failed for {PlayerName}.", characterId, player.Name);
            }
        }

        public void SaveCharacter(Character character)
        {
            SaveModel(character.GetRawCharacterModel());
            character.ProcessChanges(false);
        }

        public void SaveCharacterPreferences(CharacterPreferences characterPreferences)
        {
            SaveModel(characterPreferences.GetRawCharacterPreferencesModel());
            characterPreferences.ProcessChanges(false);
        }

        /// <summary>
        /// Saves every loaded character and preferences component that has unsaved changes.
        /// </summary>
        public void SaveChangedCharacters()
        {
            foreach (Character character in _entityManager.GetComponents<Character>().Where(c => c.HasChanges()))
            {
                TrySave(() => SaveCharacter(character), character.GetId());
            }

            foreach (CharacterPreferences preferences in _entityManager.GetComponents<CharacterPreferences>().Where(p => p.HasChanges()))
            {
                TrySave(() => SaveCharacterPreferences(preferences), preferences.GetRawCharacterPreferencesModel().CharacterId);
            }
        }

        private void UnloadCharacter(Player player)
        {
            Character? currentCharacter = player.GetComponent<Character>();
            if (currentCharacter != null && currentCharacter.HasChanges())
            {
                SaveCharacter(currentCharacter);
            }

            CharacterPreferences? currentPreferences = player.GetComponent<CharacterPreferences>();
            if (currentPreferences != null && currentPreferences.HasChanges())
            {
                SaveCharacterPreferences(currentPreferences);
            }

            player.DestroyComponents<Character>();
            player.DestroyComponents<CharacterPreferences>();
        }

        private CharacterModel? GetCharacterModelById(ulong characterId)
        {
            return _dataContext.Characters
                .AsNoTracking()
                .FirstOrDefault(character => character.Id == characterId);
        }

        private CharacterPreferencesModel GetOrCreateCharacterPreferencesModel(ulong characterId)
        {
            CharacterPreferencesModel? preferences = _dataContext.CharacterPreferences
                .AsNoTracking()
                .FirstOrDefault(p => p.CharacterId == characterId);

            if (preferences != null)
            {
                return preferences;
            }

            CharacterPreferencesModel newPreferences = new CharacterPreferencesModel { CharacterId = characterId };
            SaveModel(newPreferences);
            return newPreferences;
        }

        /// <summary>
        /// Makes sure the character's inventory component exists before the character is played.
        /// </summary>
        private void EnsureCharacterHasLoadedInventory(CharacterModel characterModel)
        {
            if (characterModel.InventoryId != null)
            {
                // A new character's inventory is inserted by BaseDataContext, straight into the database; load it now
                // instead of waiting for the next inventory sync.
                _inventoryManager.LoadInventory(characterModel.InventoryId.Value);
                return;
            }

            CreateCharacterInventory(characterModel);
        }

        /// <summary>
        /// Characters created before inventories were added automatically may not have one yet.
        /// </summary>
        private void CreateCharacterInventory(CharacterModel characterModel)
        {
            string inventoryName = $"{characterModel.FirstName} {characterModel.LastName}`s Inventory";
            Inventory inventory = _inventoryManager.CreateInventory(inventoryName, CharacterInventoryMaxWeightInGrams);

            characterModel.InventoryId = inventory.GetId();
            SaveModel(characterModel);
        }

        private async Task PublishCharacterSelected(Player player, Character character)
        {
            OnCharacterSelectedEventArgs eventArgs = new OnCharacterSelectedEventArgs
            {
                Player = player,
                Account = player.GetComponent<Account>(),
                Character = character
            };
            await _serverEventAggregator.PublishAsync(eventArgs);
        }

        /// <summary>
        /// Inserts or updates a model that is not tracked, then stops tracking it again so a later load
        /// of the same row in this context can't conflict with it.
        /// </summary>
        private void SaveModel(BaseModel model)
        {
            _dataContext.Update(model);
            _dataContext.SaveChanges();
            _dataContext.Entry(model).State = EntityState.Detached;
        }

        private void TrySave(Action save, ulong characterId)
        {
            try
            {
                save();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not save character {CharacterId}.", characterId);
            }
        }
    }
}
