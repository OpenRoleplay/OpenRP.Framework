using OpenRP.Framework.Features.Currencies.Entities;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Currencies.Components
{
    /// <summary>
    /// A hand-to-hand payment, attached to the sender. Once accepted, the bills are handed over one denomination at a
    /// time: the sender hands a bill over, then the receiver takes it, then the next bill.
    /// </summary>
    public class PendingPayment : Component
    {
        private readonly Player _receiver;
        private readonly ulong _currencyId;
        private readonly decimal _amount;
        private readonly List<BillStack> _bills;
        private int _billIndex;
        private bool _isAccepted;
        private bool _isReceiverTurn;

        public PendingPayment(Player receiver, ulong currencyId, decimal amount, List<BillStack> bills)
        {
            _receiver = receiver;
            _currencyId = currencyId;
            _amount = amount;
            _bills = bills;
        }

        public Player GetSender()
        {
            return GetComponent<Player>();
        }

        public Player GetReceiver()
        {
            return _receiver;
        }

        /// <summary>
        /// True when the player is the sender or the receiver of this payment.
        /// </summary>
        public bool IsPlayerInvolved(Player player)
        {
            return GetSender() == player || GetReceiver() == player;
        }

        /// <summary>
        /// The other side of the payment: the receiver for the sender, the sender for the receiver.
        /// </summary>
        public Player GetOtherPlayer(Player player)
        {
            return GetSender() == player ? GetReceiver() : GetSender();
        }

        public ulong GetCurrencyId()
        {
            return _currencyId;
        }

        public decimal GetAmount()
        {
            return _amount;
        }

        public bool IsAccepted()
        {
            return _isAccepted;
        }

        public void Accept()
        {
            _isAccepted = true;
        }

        public bool IsReceiverTurn()
        {
            return _isReceiverTurn;
        }

        public bool IsFirstBill()
        {
            return _billIndex == 0;
        }

        public bool HasMoreBills()
        {
            return _billIndex < _bills.Count;
        }

        public BillStack GetCurrentBill()
        {
            return _bills[_billIndex];
        }

        /// <summary>
        /// Passes the turn to the other player. After the receiver's turn the next bill comes up.
        /// </summary>
        public void EndTurn()
        {
            if (_isReceiverTurn)
            {
                _billIndex++;
            }
            _isReceiverTurn = !_isReceiverTurn;
        }
    }
}
