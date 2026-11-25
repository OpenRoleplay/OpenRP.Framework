using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Banking.Components;
using OpenRP.Framework.Features.Banking.Enums;
using SampSharp.Entities.SAMP;
using System;

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

        /// <summary>
        /// The ATM: the given card, or one of the cards the player carries, then its PIN, then balance, withdrawing and
        /// depositing.
        /// </summary>
        void OpenAtm(Player player, Atm atm, ulong? cardId);

        /// <summary>
        /// A card's details from the inventory, where whoever holds it can rename it.
        /// </summary>
        void OpenCard(Player player, ulong cardId);

        /// <summary>
        /// Lets the player choose a card they carry in this currency and type its PIN. Calls back with the card when the
        /// PIN is correct; cancelling, a blocked card or no card calls <paramref name="onCancelled"/>. A disconnect
        /// calls neither.
        /// </summary>
        void OpenCardAuthorisation(Player player, ulong currencyId, string title, Action<ulong> onAuthorised, Action onCancelled);
    }
}
