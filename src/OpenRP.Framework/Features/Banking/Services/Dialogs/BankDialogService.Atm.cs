using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Banking.Components;
using OpenRP.Framework.Features.Banking.Entities;
using OpenRP.Framework.Features.Banking.Helpers;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenRP.Framework.Features.Banking.Services.Dialogs
{
    /// <summary>
    /// The ATM: a card and its PIN, then balance, withdrawing and depositing. Every step checks that the player is
    /// still at the ATM and the card still works.
    /// </summary>
    public partial class BankDialogService
    {
        private const string AtmTitle = "ATM";

        public void OpenAtm(Player player, Atm atm, ulong? cardId)
        {
            if (!_atmService.IsPlayerAtAtm(player, atm))
            {
                return;
            }

            Action<BankCardDetails> onCardChosen = card => AskPin(player, card, AtmTitle, () => BeginAtmSession(player, atm, card.Card.Id), () => { });
            BankCardDetails? givenCard = cardId == null ? null : _bankCardService.GetCardDetails(cardId.Value);
            if (givenCard != null)
            {
                onCardChosen(givenCard);
                return;
            }
            ChooseCarriedCard(player, null, AtmTitle, onCardChosen, () => { });
        }

        private void BeginAtmSession(Player player, Atm atm, ulong cardId)
        {
            player.DestroyComponents<AtmSession>();
            player.AddComponent<AtmSession>(cardId, atm.Entity);
            ShowAtmMenu(player);
        }

        private void EndAtmSession(Player player)
        {
            if (player.IsComponentAlive)
            {
                player.DestroyComponents<AtmSession>();
            }
        }

        private void ShowAtmMenu(Player player)
        {
            WithAtm(player, (atm, card) =>
            {
                BetterListDialog atmDialog = new BetterListDialog("Choose", "Take card");
                atmDialog.SetTitle(TitleType.Children, AtmTitle, card.GetLabel());
                int balanceRow = atmDialog.AddRow("Balance");
                int withdrawRow = atmDialog.AddRow("Withdraw cash");
                int depositRow = atmDialog.AddRow("Deposit cash");

                _dialogService.ShowIgnoringDisconnect(player, atmDialog, response =>
                {
                    if (response.Response != DialogResponse.LeftButton)
                        EndAtmSession(player);
                    else if (response.ItemIndex == balanceRow)
                        ShowAtmBalance(player);
                    else if (response.ItemIndex == withdrawRow)
                        OpenAtmWithdrawal(player);
                    else if (response.ItemIndex == depositRow)
                        OpenAtmDeposit(player);
                    else
                        ShowAtmMenu(player);
                });
            });
        }

        /// <summary>
        /// Runs the step when the player still has an ATM session at a nearby ATM with a working card; otherwise ends
        /// the session and says why.
        /// </summary>
        private void WithAtm(Player player, Action<Atm, BankCardDetails> step)
        {
            AtmSession? session = player.IsComponentAlive ? player.GetComponent<AtmSession>() : null;
            if (session == null)
            {
                return;
            }

            Atm? atm = session.GetAtm();
            BankCardDetails? card = _bankCardService.GetCardDetails(session.GetCardId());
            string? error = atm == null || !_atmService.IsPlayerAtAtm(player, atm)
                ? "You walked away from the ATM."
                : card == null ? "The ATM doesn't recognise this card." : _bankCardService.GetUseError(player, card);
            if (error != null)
            {
                EndAtmSession(player);
                ShowMessage(player, AtmTitle, "Not possible", error, () => { });
                return;
            }
            step(atm!, card!);
        }

        private void ShowAtmBalance(Player player)
        {
            WithAtm(player, (atm, card) =>
            {
                BankAccountModel? account = _bankAccountService.GetAccount(card.Account.Id);
                if (account == null)
                {
                    ShowAtmMenu(player);
                    return;
                }

                StringBuilder text = new StringBuilder();
                text.AppendLine($"Balance: {Highlight(FormatMoney(account.CurrencyId, account.Balance))}");
                text.AppendLine(GetPendingDepositsText(account));
                text.AppendLine($"You can still withdraw {Highlight(FormatMoney(account.CurrencyId, _atmService.GetRemainingWithdrawal(card.Card.Id)))} with this card right now.");
                ShowMessage(player, AtmTitle, "Balance", text.ToString(), () => ShowAtmMenu(player));
            });
        }

        private void OpenAtmWithdrawal(Player player)
        {
            WithAtm(player, (atm, card) =>
            {
                decimal remaining = _atmService.GetRemainingWithdrawal(card.Card.Id);
                string prompt = $"How much? You can still withdraw {Highlight(FormatMoney(card.Account.CurrencyId, remaining))} with this card right now.";
                AskAtmAmount(player, "Withdraw", prompt, amount => DoAtmWithdrawal(player, amount));
            });
        }

        private void DoAtmWithdrawal(Player player, decimal amount)
        {
            WithAtm(player, (atm, card) => _ = RunAsync(player, async () =>
            {
                string? error = await _atmService.WithdrawAsync(player, card.Card.Id, amount);
                if (!player.IsComponentAlive)
                {
                    return;
                }
                string doneText = $"Take your cash: {Highlight(FormatMoney(card.Account.CurrencyId, amount))}.";
                ShowMessage(player, AtmTitle, error == null ? "Withdraw" : "Not possible", error ?? doneText, () => ShowAtmMenu(player));
            }));
        }

        private void OpenAtmDeposit(Player player)
        {
            string prompt = $"How much cash do you put in? An ATM deposit reaches the account after {BankingRules.AtmDepositDelay.TotalHours:0} hours; at a bank teller it's there at once.";
            AskAtmAmount(player, "Deposit", prompt, amount => DoAtmDeposit(player, amount));
        }

        private void DoAtmDeposit(Player player, decimal amount)
        {
            WithAtm(player, (atm, card) => _ = RunAsync(player, async () =>
            {
                string? error = await _atmService.DepositAsync(player, card.Card.Id, atm, amount);
                if (!player.IsComponentAlive)
                {
                    return;
                }
                DateTime arrivesAt = DateTime.UtcNow + BankingRules.AtmDepositDelay;
                string doneText = $"The ATM took {Highlight(FormatMoney(card.Account.CurrencyId, amount))}. It reaches {card.Account.Name} at about {arrivesAt.ToLocalTime():HH:mm}.";
                ShowMessage(player, AtmTitle, error == null ? "Deposit" : "Not possible", error ?? doneText, () => ShowAtmMenu(player));
            }));
        }

        private void AskAtmAmount(Player player, string subtitle, string prompt, Action<decimal> onEntered)
        {
            void OnEntered(string text)
            {
                decimal? amount = BankAmounts.Parse(text);
                if (amount == null)
                {
                    ShowMessage(player, AtmTitle, subtitle, "Enter an amount above 0, with at most 2 decimals, for example 250 or 99.50.",
                        () => AskAtmAmount(player, subtitle, prompt, onEntered));
                    return;
                }
                onEntered(amount.Value);
            }

            OpenTextInput(player, AtmTitle, subtitle, prompt, OnEntered, () => ShowAtmMenu(player));
        }

        /// <summary>
        /// A row on the teller's account menu when ATM deposits are on their way, or null.
        /// </summary>
        private int? AddPendingDepositsRow(BetterListDialog dialog, BankAccountModel account)
        {
            List<BankPendingDepositModel> pending = _atmService.GetPendingDeposits(account.Id);
            return pending.Count == 0 ? null : dialog.AddRow($"Pending ATM deposits: {Highlight(FormatMoney(account.CurrencyId, pending.Sum(deposit => deposit.Amount)))}");
        }

        private void ShowPendingDeposits(Player player, BankAccountModel account, Action onBack)
        {
            ShowMessage(player, account.Name, "Pending ATM deposits", GetPendingDepositsText(account), onBack);
        }

        private string GetPendingDepositsText(BankAccountModel account)
        {
            List<BankPendingDepositModel> pending = _atmService.GetPendingDeposits(account.Id);
            if (pending.Count == 0)
            {
                return "No ATM deposits on their way.";
            }

            StringBuilder text = new StringBuilder("ATM deposits on their way:\n");
            foreach (BankPendingDepositModel deposit in pending)
            {
                text.AppendLine($"  {FormatMoney(account.CurrencyId, deposit.Amount)}  {ChatColor.LightGray}arrives at {deposit.CreditAt.ToLocalTime():dd MMM HH:mm}{ChatColor.White}");
            }
            return text.ToString();
        }
    }
}
