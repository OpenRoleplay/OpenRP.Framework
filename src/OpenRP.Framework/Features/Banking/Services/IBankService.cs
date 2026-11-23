using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Actors.Components;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Banking.Services
{
    /// <summary>
    /// Banks, their branches and their tellers.
    /// </summary>
    public interface IBankService
    {
        List<BankModel> GetBanks();

        BankModel? GetBank(ulong bankId);

        /// <summary>
        /// The highest monthly fee the bank may charge per account: the cap its jurisdiction has set, or the default
        /// when the jurisdiction can't be found.
        /// </summary>
        decimal GetMaxMonthlyFee(BankModel bank);

        /// <summary>
        /// An amount of money (a fee, or the cap) as menu text, in the currency of the bank's jurisdiction. Falls back
        /// to the plain amount when the jurisdiction can't be found.
        /// </summary>
        string FormatFee(BankModel bank, decimal amount);

        /// <summary>
        /// The bank the actor works for as a teller, or null.
        /// </summary>
        BankModel? GetTellerBank(ServerActor actor);

        /// <summary>
        /// Whether the actor stands inside one of the bank's branch properties.
        /// </summary>
        bool IsInsideBranch(ServerActor actor, ulong bankId);

        List<BankBranchModel> GetBranches(ulong bankId);

        /// <summary>
        /// The bank the property is a branch of, or null.
        /// </summary>
        BankModel? GetBankOfBranch(ulong propertyId);

        List<BankTellerModel> GetTellers(ulong bankId);
    }
}
