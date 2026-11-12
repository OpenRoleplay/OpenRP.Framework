using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Jurisdictions.Components;
using OpenRP.Framework.Features.Properties.Components;

namespace OpenRP.Framework.Features.Banking.Services
{
    /// <summary>
    /// What admins change in /editbanks. Methods return why it isn't possible, or null.
    /// </summary>
    public interface IBankEditorService
    {
        /// <summary>
        /// Creates the bank and its house account, which receives its fees.
        /// </summary>
        (BankModel? Bank, string? Error) CreateBank(string name, string code, Jurisdiction jurisdiction);

        string? RenameBank(BankModel bank, string name);

        string? SetMonthlyFee(BankModel bank, string feeText);

        void SetRunByFaction(BankModel bank, Faction? faction);

        string? AddBranch(BankModel bank, Property property);

        void RemoveBranch(ulong branchId);

        string? AddTeller(BankModel bank, ServerActor actor);

        void RemoveTeller(ulong tellerId);

        (BankAccountModel? Account, string? Error) OpenPropertyAccount(BankModel bank, Property property, string name);
    }
}
