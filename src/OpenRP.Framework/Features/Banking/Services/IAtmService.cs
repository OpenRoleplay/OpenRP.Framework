using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Banking.Components;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Banking.Services
{
    /// <summary>
    /// ATMs: where they are, cash in and out with a card, and deposits that reach the account later. Methods that
    /// move money return why it didn't happen, or null. The card's PIN must have been checked before.
    /// </summary>
    public interface IAtmService
    {
        /// <summary>
        /// Makes the map object on this entity an ATM.
        /// </summary>
        void AddAtm(EntityId objectEntity, EntityId mapObjectEntity);

        /// <summary>
        /// The ATM the player stands at, or null.
        /// </summary>
        Atm? GetNearbyAtm(Player player);

        bool IsPlayerAtAtm(Player player, Atm atm);

        /// <summary>
        /// How much the card may still take from ATMs now, within the limit per period.
        /// </summary>
        decimal GetRemainingWithdrawal(ulong cardId);

        /// <summary>
        /// Takes the money from the card's account, within the limit, and gives the player the cash.
        /// </summary>
        Task<string?> WithdrawAsync(Player player, ulong cardId, decimal amount);

        /// <summary>
        /// Takes the cash from the player now; it reaches the card's account after the ATM deposit delay.
        /// </summary>
        Task<string?> DepositAsync(Player player, ulong cardId, Atm atm, decimal amount);

        /// <summary>
        /// The account's ATM deposits that haven't reached it yet, soonest first.
        /// </summary>
        List<BankPendingDepositModel> GetPendingDeposits(ulong accountId);

        /// <summary>
        /// Puts every ATM deposit that's due on its account.
        /// </summary>
        void CreditDueDeposits();
    }
}
