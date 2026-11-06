using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Accounts.Components;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Accounts.Services
{
    public interface IAccountService
    {
        bool DoesAccountExist(string username);
        List<AccountModel>? GetAccountsByCharacterName(string characterName);
        AccountModel? GetAccountByUsername(string username);
        bool CheckPassword(string username, string password);
        /// <summary>
        /// Logs the player into the account, unless it's already logged in somewhere. Returns that error, or null.
        /// </summary>
        Task<string?> LoginPlayer(Player player, string username);
        bool CreateAccount(Player player, string username, string password);
    }
}
