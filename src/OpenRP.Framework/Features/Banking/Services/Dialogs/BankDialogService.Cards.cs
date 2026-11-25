using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Banking.Entities;
using OpenRP.Framework.Features.Banking.Enums;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using SampSharp.Entities.SAMP;
using System;

namespace OpenRP.Framework.Features.Banking.Services.Dialogs
{
    public partial class BankDialogService
    {
        private const string CardTitle = "Debit card";

        /// <summary>
        /// The player's own card for this account at the teller: ordering one, or renaming, changing the PIN, blocking,
        /// unblocking and replacing it.
        /// </summary>
        private void OpenCardMenu(Player player, ServerActor teller, ulong accountId, Action onBack)
        {
            BankAccountModel? account = _bankAccountService.GetAccount(accountId);
            if (account == null || !player.IsPlayerPlayingAsCharacter())
            {
                onBack();
                return;
            }
            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            BankCardDetails? card = _bankCardService.GetCurrentCard(accountId, character.GetId());

            BetterListDialog cardDialog = new BetterListDialog("Choose", "Back");
            cardDialog.SetTitle(TitleType.Children, "Bank", account.Name, CardTitle);
            if (card != null)
            {
                cardDialog.AddRow($"{card.GetLabel()}  {Highlight(card.GetStatusText())}");
            }
            int? renameRow = card == null ? null : cardDialog.AddRow("Rename the card");
            int? pinRow = card == null ? null : cardDialog.AddRow("Change the PIN");
            int? blockRow = card == null || card.Card.BlockedAt != null ? null : cardDialog.AddRow("Block the card");
            int? unblockRow = card?.Card.BlockedAt == null ? null : cardDialog.AddRow("Unblock the card");
            int orderRow = cardDialog.AddRow(card == null ? "Order a card" : $"Order a new card (the card ...{card.Card.Digits} stops working)");

            _dialogService.ShowIgnoringDisconnect(player, cardDialog, response =>
            {
                Action reopen = () => OpenCardMenu(player, teller, accountId, onBack);
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == orderRow)
                    OpenOrderCard(player, teller, account, reopen);
                else if (card == null)
                    reopen();
                else if (response.ItemIndex == renameRow)
                    OpenRenameCard(player, card.Card.Id, () => WithTeller(player, teller, BankOperation.ManageCards, reopen), reopen);
                else if (response.ItemIndex == pinRow)
                    OpenChangePin(player, teller, card.Card.Id, reopen);
                else if (response.ItemIndex == blockRow)
                    OpenBlockCard(player, teller, card, reopen);
                else if (response.ItemIndex == unblockRow)
                    DoCardChange(player, teller, () => _bankCardService.UnblockCard(card.Card.Id), "The card works again.", reopen);
                else
                    reopen();
            });
        }

        /// <summary>
        /// An optional name, then the PIN twice.
        /// </summary>
        private void OpenOrderCard(Player player, ServerActor teller, BankAccountModel account, Action onBack)
        {
            void OnNameEntered(string name)
            {
                string? error = _bankCardService.GetCardNameError(name);
                if (error != null)
                {
                    ShowMessage(player, CardTitle, "Name", error, () => OpenOrderCard(player, teller, account, onBack));
                    return;
                }
                AskNewPin(player, pin => DoOrderCard(player, teller, account.Id, name, pin, onBack), onBack);
            }

            OpenTextInput(player, account.Name, "Order a card",
                $"Give the card a name if you like, for example \"Groceries\". Leave it empty for none. At most {BankingRules.CardNameMaxLength} characters.", OnNameEntered, onBack);
        }

        private void DoOrderCard(Player player, ServerActor teller, ulong accountId, string name, string pin, Action onBack)
        {
            WithTeller(player, teller, BankOperation.ManageCards, () => _ = RunAsync(player, async () =>
            {
                string? error = await _bankCardService.IssueCardAsync(player, accountId, name, pin);
                if (!player.IsComponentAlive)
                {
                    return;
                }
                ContinueAfter(player, CardTitle, error,
                    () => ShowMessage(player, CardTitle, "Your card", "Here's your new card. It's in your inventory. Keep the PIN to yourself.", onBack), onBack);
            }));
        }

        private void OpenChangePin(Player player, ServerActor teller, ulong cardId, Action onBack)
        {
            AskNewPin(player, pin => WithTeller(player, teller, BankOperation.ManageCards, () => _ = RunAsync(player, async () =>
            {
                string? error = await _bankCardService.ChangePinAsync(cardId, pin);
                if (!player.IsComponentAlive)
                {
                    return;
                }
                ContinueAfter(player, CardTitle, error, () => ShowMessage(player, CardTitle, "PIN", "The card has its new PIN.", onBack), onBack);
            })), onBack);
        }

        private void OpenBlockCard(Player player, ServerActor teller, BankCardDetails card, Action onBack)
        {
            ShowConfirmation(player, CardTitle, "Block",
                $"Block {card.GetLabel()}? Nobody can use it until the bank unblocks it. Block it when it's lost or stolen.",
                () => DoCardChange(player, teller, () => _bankCardService.BlockCard(card.Card.Id), "The card is blocked.", onBack), onBack, "Block", "Back");
        }

        private void DoCardChange(Player player, ServerActor teller, Func<string?> change, string doneText, Action onBack)
        {
            WithTeller(player, teller, BankOperation.ManageCards, () =>
            {
                string? error = change();
                ContinueAfter(player, CardTitle, error, () => ShowMessage(player, CardTitle, "Done", doneText, onBack), onBack);
            });
        }

        /// <summary>
        /// The new PIN twice, as password input, so a typing mistake doesn't lock the player out.
        /// </summary>
        private void AskNewPin(Player player, Action<string> onEntered, Action onBack)
        {
            void OnFirstEntered(string pin)
            {
                string? error = _bankCardService.GetPinError(pin);
                if (error != null)
                {
                    ShowMessage(player, CardTitle, "PIN", error, () => AskNewPin(player, onEntered, onBack));
                    return;
                }
                OpenPinInput(player, CardTitle, "PIN", "Type the PIN again:", repeated =>
                {
                    if (repeated != pin)
                    {
                        ShowMessage(player, CardTitle, "PIN", "The two PINs weren't the same. Try again.", () => AskNewPin(player, onEntered, onBack));
                        return;
                    }
                    onEntered(pin);
                }, onBack);
            }

            OpenPinInput(player, CardTitle, "PIN", $"Choose a PIN of {BankingRules.PinLength} digits:", OnFirstEntered, onBack);
        }

        private void OpenPinInput(Player player, string title, string subtitle, string prompt, Action<string> onEntered, Action onBack)
        {
            BetterInputDialog pinDialog = new BetterInputDialog("Continue", "Back");
            pinDialog.SetTitle(TitleType.Children, title, subtitle);
            pinDialog.SetContent(prompt);
            pinDialog.IsPassword = true;
            _dialogService.ShowIgnoringDisconnect(player, pinDialog, response =>
            {
                if (response.Response != DialogResponse.LeftButton)
                {
                    onBack();
                    return;
                }
                onEntered(response.InputText ?? string.Empty);
            });
        }

        /// <summary>
        /// A new name for the card, or none. Whoever holds the card may rename it, like writing on a label.
        /// </summary>
        private void OpenRenameCard(Player player, ulong cardId, Action onDone, Action onBack)
        {
            void OnEntered(string name)
            {
                string? error = _bankCardService.RenameCard(cardId, name);
                ContinueAfter(player, CardTitle, error, onDone, () => OpenRenameCard(player, cardId, onDone, onBack));
            }

            OpenTextInput(player, CardTitle, "Rename",
                $"Enter a name for the card, for example \"Groceries\", or leave it empty to remove the name. At most {BankingRules.CardNameMaxLength} characters.", OnEntered, onBack);
        }
    }
}
