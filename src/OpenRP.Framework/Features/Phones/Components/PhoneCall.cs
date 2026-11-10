using OpenRP.Framework.Features.Phones.Entities;
using OpenRP.Framework.Features.Phones.Enums;
using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.Phones.Components
{
    /// <summary>
    /// A call in progress, on its own entity that only lives as long as the call. The caller is the player who dialed;
    /// the other side is another phone, or a server number such as 911.
    /// </summary>
    public class PhoneCall : Component
    {
        private readonly ulong _callerPhoneId;
        private readonly EntityId _callerPlayer;
        private readonly uint _calledNumber;
        private readonly ulong? _calledPhoneId;
        private readonly IServerPhoneNumber? _serverNumber;
        private readonly DateTime _startedAt;
        private PhoneCallState _state;
        private DateTime? _connectedAt;
        private EntityId _calledPlayer;
        private DateTime _lastRingAt;

        public PhoneCall(ulong callerPhoneId, EntityId callerPlayer, uint calledNumber, ulong? calledPhoneId, IServerPhoneNumber? serverNumber)
        {
            _callerPhoneId = callerPhoneId;
            _callerPlayer = callerPlayer;
            _calledNumber = calledNumber;
            _calledPhoneId = calledPhoneId;
            _serverNumber = serverNumber;
            _startedAt = DateTime.UtcNow;
            _lastRingAt = DateTime.MinValue;
            _state = PhoneCallState.Ringing;
        }

        public ulong GetCallerPhoneId()
        {
            return _callerPhoneId;
        }

        public EntityId GetCallerPlayer()
        {
            return _callerPlayer;
        }

        public uint GetCalledNumber()
        {
            return _calledNumber;
        }

        /// <summary>
        /// The phone being called, or null for a server number.
        /// </summary>
        public ulong? GetCalledPhoneId()
        {
            return _calledPhoneId;
        }

        /// <summary>
        /// The server number being called, such as 911, or null for a player's phone.
        /// </summary>
        public IServerPhoneNumber? GetServerNumber()
        {
            return _serverNumber;
        }

        public bool IsToServerNumber()
        {
            return _serverNumber != null;
        }

        public DateTime GetStartedAt()
        {
            return _startedAt;
        }

        public bool IsRinging()
        {
            return _state == PhoneCallState.Ringing;
        }

        public bool IsConnected()
        {
            return _state == PhoneCallState.Connected;
        }

        /// <summary>
        /// The player who answered, or EntityId.Empty while it rings and for server numbers.
        /// </summary>
        public EntityId GetCalledPlayer()
        {
            return _calledPlayer;
        }

        /// <summary>
        /// Picks up the call. The answering player is empty for a server number.
        /// </summary>
        public void Connect(EntityId calledPlayer)
        {
            _state = PhoneCallState.Connected;
            _connectedAt = DateTime.UtcNow;
            _calledPlayer = calledPlayer;
        }

        /// <summary>
        /// How long the two sides have been talking, or 0 when nobody picked up.
        /// </summary>
        public int GetDurationSeconds()
        {
            return _connectedAt.HasValue ? (int)(DateTime.UtcNow - _connectedAt.Value).TotalSeconds : 0;
        }

        public DateTime GetLastRingAt()
        {
            return _lastRingAt;
        }

        public void SetLastRingAt(DateTime lastRingAt)
        {
            _lastRingAt = lastRingAt;
        }
    }
}
