using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Banking.Enums;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Banking.Services.Dialogs
{
    public interface IBankDialogService
    {
        /// <summary>
        /// The full bank menu at this teller: the player's accounts at its bank, and opening one.
        /// </summary>
        void OpenBankMenu(Player player, ServerActor teller);

        /// <summary>
        /// Goes straight to one operation, as a teller does when a customer asks for it. Operations on an account start
        /// by picking the account when the player has several. A suggested amount is only shown for confirmation.
        /// </summary>
        void OpenOperation(Player player, ServerActor teller, BankOperation operation, decimal? suggestedAmount);

        /// <summary>
        /// /editbanks: banks, their fees, branches and tellers, property accounts, and looking up an OBAN.
        /// </summary>
        void OpenBankEditor(Player player);
    }
}
