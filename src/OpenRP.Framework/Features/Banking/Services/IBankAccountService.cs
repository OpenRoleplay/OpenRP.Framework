using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Banking.Entities;
using OpenRP.Framework.Features.Banking.Enums;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Factions.Components;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Banking.Services
{
    /// <summary>
    /// Bank accounts: who may do what with them, opening and closing, access lists and history. Money moves through
    /// <see cref="IBankTransactionService"/>.
    /// </summary>
    public interface IBankAccountService
    {
        BankAccountModel? GetAccount(ulong accountId);

        /// <summary>
        /// The account with this OBAN (without spaces), open or closed, or null.
        /// </summary>
        BankAccountModel? GetAccountByOban(string oban);

        /// <summary>
        /// What the character may do with the account. A closed account allows nothing.
        /// </summary>
        BankAccessLevel GetAccessLevel(Character character, BankAccountModel account);

        /// <summary>
        /// The open accounts at the bank the character has at least this access to, by name.
        /// </summary>
        List<BankAccountModel> GetAccountsAt(Character character, ulong bankId, BankAccessLevel minimumLevel);

        /// <summary>
        /// Who owns the account, as a name: a character, a faction, a property or the bank itself.
        /// </summary>
        string GetOwnerName(BankAccountModel account);

        /// <summary>
        /// The factions the character may open accounts for (faction.bank).
        /// </summary>
        List<Faction> GetFactionsToBankFor(Character character);

        /// <summary>
        /// Opens an account at the bank, in its jurisdiction's currency, with a new OBAN. The opener must be allowed to
        /// open it for that owner; property and bank owners are for admins (pass null as the opener).
        /// </summary>
        (BankAccountModel? Account, string? Error) OpenAccount(Character? opener, BankModel bank, BankAccountOwnerType ownerType, ulong ownerId, string name);

        /// <summary>
        /// Closes the account, when the character manages it and its balance is exactly 0.
        /// </summary>
        string? CloseAccount(Character character, BankAccountModel account);

        /// <summary>
        /// The bank's own account in this currency, which receives the fees of accounts in it; opened when it doesn't
        /// exist yet, for example after the jurisdiction changed its currency. Null when it couldn't be opened.
        /// </summary>
        BankAccountModel? GetOrOpenHouseAccount(BankModel bank, ulong currencyId);

        List<BankAccessEntry> GetAccessList(BankAccountModel account);

        /// <summary>
        /// Gives the character with this exact name access, or changes their level.
        /// </summary>
        string? GrantAccess(Character manager, BankAccountModel account, string characterName, BankAccessLevel level);

        string? RevokeAccess(Character manager, BankAccountModel account, ulong accessId);

        /// <summary>
        /// The newest transactions first, at most <see cref="BankingRules.HistoryLength"/>.
        /// </summary>
        List<BankTransactionModel> GetHistory(BankAccountModel account);
    }
}
