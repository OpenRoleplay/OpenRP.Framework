using OpenRP.Framework.Features.Banking.Entities;
using OpenRP.Framework.Features.Banking.Enums;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Banking.Services.Dialogs
{
    /// <summary>
    /// Using a card: choosing one of the cards the player carries, typing its PIN, and the card's own menu from the
    /// inventory.
    /// </summary>
    public partial class BankDialogService
    {
        public void OpenCardAuthorisation(Player player, ulong currencyId, string title, Action<ulong> onAuthorised, Action onCancelled)
        {
            ChooseCarriedCard(player, currencyId, title, card => AskPin(player, card, title, () => onAuthorised(card.Card.Id), onCancelled), onCancelled);
        }

        public void OpenCard(Player player, ulong cardId)
        {
            BankCardDetails? card = _bankCardService.GetCardDetails(cardId);
            if (card == null)
            {
                return;
            }

            BetterListDialog cardDialog = new BetterListDialog("Choose", "Close");
            cardDialog.SetTitle(TitleType.Children, CardTitle);
            cardDialog.AddRow($"Bank: {Highlight(card.Bank.Name)}");
            cardDialog.AddRow($"Account: {Highlight(card.Account.Name)}");
            cardDialog.AddRow($"Card number: {Highlight($"...{card.Card.Digits}")}");
            cardDialog.AddRow($"Status: {Highlight(card.GetStatusText())}");
            int renameRow = cardDialog.AddRow(string.IsNullOrEmpty(card.Card.Name) ? "Give the card a name" : $"Rename the card ({card.Card.Name})");

            _dialogService.ShowIgnoringDisconnect(player, cardDialog, response =>
            {
                Action reopen = () => OpenCard(player, cardId);
                if (response.Response != DialogResponse.LeftButton)
                    return;
                if (response.ItemIndex == renameRow)
                    OpenRenameCard(player, cardId, reopen, reopen);
                else
                    reopen();
            });
        }

        /// <summary>
        /// One of the cards the player carries, also in wallets, in the currency when one is given. With one card, it's
        /// taken at once.
        /// </summary>
        private void ChooseCarriedCard(Player player, ulong? currencyId, string title, Action<BankCardDetails> onChosen, Action onBack)
        {
            List<BankCardDetails> cards = GetCarriedCards(player, currencyId);
            if (cards.Count == 0)
            {
                ShowMessage(player, title, "No card", currencyId == null ? "You don't carry a debit card." : "You don't carry a debit card that pays in this currency.", onBack);
                return;
            }
            if (cards.Count == 1)
            {
                onChosen(cards[0]);
                return;
            }

            BetterListDialog cardDialog = new BetterListDialog("Choose", "Back");
            cardDialog.SetTitle(TitleType.Children, title, "Which card?");
            Dictionary<int, BankCardDetails> cardsByRow = new Dictionary<int, BankCardDetails>();
            foreach (BankCardDetails card in cards)
            {
                string status = card.GetStatus() == BankCardStatus.Active ? string.Empty : $"  {Highlight(card.GetStatusText())}";
                cardsByRow[cardDialog.AddRow(card.GetLabel() + status)] = card;
            }

            _dialogService.ShowIgnoringDisconnect(player, cardDialog, response =>
            {
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (cardsByRow.TryGetValue(response.ItemIndex, out BankCardDetails? card))
                    onChosen(card);
            });
        }

        private List<BankCardDetails> GetCarriedCards(Player player, ulong? currencyId)
        {
            if (!player.IsPlayerPlayingAsCharacter())
            {
                return new List<BankCardDetails>();
            }
            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            return _bankCardService.GetCarriedCardItems(character)
                .Select(cardItem => cardItem.GetBankCardId())
                .OfType<ulong>()
                .Distinct()
                .Select(cardId => _bankCardService.GetCardDetails(cardId))
                .OfType<BankCardDetails>()
                .Where(card => currencyId == null || card.Account.CurrencyId == currencyId)
                .ToList();
        }

        /// <summary>
        /// The card's PIN. A wrong PIN asks again, until the card is blocked.
        /// </summary>
        private void AskPin(Player player, BankCardDetails card, string title, Action onCorrect, Action onCancelled)
        {
            string? error = _bankCardService.GetUseError(player, card);
            if (error != null)
            {
                ShowMessage(player, title, "Card refused", error, onCancelled);
                return;
            }

            OpenPinInput(player, title, card.GetLabel(), "Enter the PIN:", pin => _ = RunAsync(player, async () =>
            {
                PinCheckResult result = await _bankCardService.CheckPinAsync(card.Card.Id, pin);
                if (!player.IsComponentAlive)
                {
                    return;
                }
                ContinueAfterPin(player, card, title, result, onCorrect, onCancelled);
            }), onCancelled);
        }

        private void ContinueAfterPin(Player player, BankCardDetails card, string title, PinCheckResult result, Action onCorrect, Action onCancelled)
        {
            if (result == PinCheckResult.Correct)
            {
                onCorrect();
                return;
            }
            if (result == PinCheckResult.Blocked)
            {
                ShowMessage(player, title, "Card blocked", "Too many wrong PINs: the card is blocked. The bank can unblock it.", onCancelled);
                return;
            }

            int attemptsLeft = BankingRules.MaxWrongPinAttempts - (_bankCardService.GetCardDetails(card.Card.Id)?.Card.WrongPinAttempts ?? 0);
            ShowMessage(player, title, "Wrong PIN", $"Wrong PIN. {attemptsLeft} more wrong and the card is blocked.", () => AskPin(player, card, title, onCorrect, onCancelled));
        }
    }
}
