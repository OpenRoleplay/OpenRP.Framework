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
