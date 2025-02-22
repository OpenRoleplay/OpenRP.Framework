using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Inventories.Services;
using SampSharp.Entities;
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
        private IEntityManager _entityManager;
        private IInventoryService _inventoryService;
        public TempCharacterService(BaseDataContext dataContext, IEntityManager entityManager, IInventoryService inventoryService)
        {
            _dataContext = dataContext;
            _entityManager = entityManager;
            _inventoryService = inventoryService;
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

        public bool SetCharacterAccent(Character character, string accent)
        {
            try
            {
                CharacterModel characterToEdit = _dataContext.Characters.FirstOrDefault(i => i.Id == character.GetDatabaseId());

                if (characterToEdit != null)
                {
                    characterToEdit.Accent = accent;

                    return _dataContext.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }

        public InventoryModel GetCharacterInventory(Character character)
        {
            try
            {
                CharacterModel characterData = _dataContext.Characters
                    .Include(c => c.Inventory)
                    .ThenInclude(c => c.Items)
                    .FirstOrDefault(c => c.Id == character.GetDatabaseId());

                // Create inventory if it doesn't exist
                if (characterData.Inventory == null)
                {
                    characterData.Inventory = _inventoryService.CreateInventory("Character Inventory", 10000);
                    _dataContext.SaveChanges();
                }

                // Transfer currency units to wallet, needs to be turned into a /organizeinv command or w/e in the future.
                /*Inventory wallet = GetCharacterWallet(character, characterData.Inventory);
                if (wallet != null)
                {
                    foreach (InventoryItemModel inventoryItem in characterData.Inventory.GetInventoryItems())
                    {
                        if (inventoryItem.GetItem().IsItemCurrency())
                        {
                            inventoryItem.Transfer(wallet.Id);
                        }
                    }
                }*/

                return characterData.Inventory;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }
    }
}
