using OpenRP.Framework.Database.Models;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Accounts.Components
{
    public class Account : Component
    {
        private AccountModel _cachedAccountModel;
        private List<CharacterModel> _cachedCharacterModels;

        public Account(AccountModel accountModel, List<CharacterModel> characterModels)
        {
            _cachedAccountModel = accountModel;
            _cachedCharacterModels = characterModels;
        }
        public ulong GetAccountId()
        {
            return _cachedAccountModel.Id;
        }

        public string GetAccountUsername()
        {
            return _cachedAccountModel.Username;
        }
    }
}
