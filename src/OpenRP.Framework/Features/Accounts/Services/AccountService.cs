using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Accounts.Components;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using OpenRP.Framework.Shared.ServerEvents.Services;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Accounts.Services
{
    public class AccountService : IAccountService
    {
        private BaseDataContext _dataContext;
        private IServerEventAggregator _serverEventAggregator;

        public AccountService(BaseDataContext context, IServerEventAggregator serverEventAggregator)
        {
            _dataContext = context;
            _serverEventAggregator = serverEventAggregator;
        }

        public bool DoesAccountExist(string username)
        {
            if (_dataContext.Accounts.Any(a => a.Username == username))
            {
                return true;
            }
            return false;
        }

        public AccountModel? GetAccountByUsername(string username)
        {
            return _dataContext.Accounts.FirstOrDefault(a => a.Username == username);
        }

        public AccountModel? GetAccountById(ulong accountId)
        {
            return _dataContext.Accounts.FirstOrDefault(a => a.Id == accountId);
        }

        public List<AccountModel>? GetAccountsByCharacterName(string characterName)
        {
            if (characterName.Contains("_"))
            {
                string[] names = characterName.Split('_');
                if (names.Length == 2)
                {
                    return _dataContext.Characters
                        .Include(a => a.Account)
                        .Where(a => a.FirstName == names[0] 
                            && a.LastName == names[1])
                        .Select(i => i.Account)
                        .ToList();
                }
            }
            return null;
        }

        public List<CharacterModel> GetCharactersByUsername(string username)
        {
            return _dataContext.Characters
                        .Include(i => i.Account)
                            .Where(i => i.Account.Username == username)
                            .ToList();
        }

        public List<CharacterModel> GetCharactersByAccountId(ulong accountId)
        {
            return _dataContext.Characters
                        .Include(i => i.Account)
                            .Where(i => i.Account.Id == accountId)
                            .ToList();
        }

        public bool CheckPassword(string username, string password)
        {
            if (DoesAccountExist(username))
            {
                AccountModel account = GetAccountByUsername(username);

                if (BCrypt.Net.BCrypt.Verify(password, account.Password))
                {
                    return true;
                }
            }
            return false;
        }

        public async Task LoginPlayer(Player player, string username)
        {
            AccountModel accountModel = GetAccountByUsername(username);
            List<CharacterModel> characterModels = GetCharactersByUsername(username);

            Account accountComponent = player.AddComponent<Account>(accountModel, characterModels);

            var eventArgs = new OnAccountLoggedInEventArgs
            {
                Player = player,
                Account = accountComponent
            };
            await _serverEventAggregator.PublishAsync(eventArgs);
        }

        public Account ReloadAccount(Player player, ulong accountId)
        {
            player.DestroyComponents<Account>();

            AccountModel accountModel = GetAccountById(accountId);
            List<CharacterModel> characterModels = GetCharactersByAccountId(accountId);

            if (accountModel != null)
            {
                return player.AddComponent<Account>(accountModel, characterModels);
            }

            return null;
        }

        public bool CreateAccount(Player player, string username, string password)
        {
            AccountCreation accountCreation = player.GetComponent<AccountCreation>();

            if (accountCreation != null)
            {
                AccountModel createdAccount = new AccountModel();
                createdAccount.Username = accountCreation.Account.Username;
                createdAccount.Password = accountCreation.Account.Password;

                _dataContext.Accounts.Add(createdAccount);
                int changes = _dataContext.SaveChanges();

                player.DestroyComponents<AccountCreation>();

                return changes > 0;
            }
            return false;
        }

        public bool CreateCharacter(Player player, CharacterModel newCharacter)
        {
            try
            {
                CharacterCreation charCreationComponent = player.GetComponent<CharacterCreation>();
                Account accountComponent = player.GetComponent<Account>();

                if (charCreationComponent != null && charCreationComponent.CreatingCharacter != null)
                {
                    newCharacter.AccountId = accountComponent.GetAccountId();
                    _dataContext.Characters.Add(newCharacter);
                    _dataContext.SaveChanges();

                    ReloadAccount(player, newCharacter.AccountId.Value);

                    return true;
                }
                else
                {
                    Console.WriteLine("There is no character to create!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }
    }
}
