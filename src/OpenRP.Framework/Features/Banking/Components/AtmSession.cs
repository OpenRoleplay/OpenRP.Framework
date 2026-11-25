using SampSharp.Entities;

namespace OpenRP.Framework.Features.Banking.Components
{
    /// <summary>
    /// On a player while they use an ATM with a card whose PIN they typed correctly. Removed when they leave the ATM menu.
    /// </summary>
    public class AtmSession : Component
    {
        private readonly ulong _cardId;
        private readonly EntityId _atmEntity;

        public AtmSession(ulong cardId, EntityId atmEntity)
        {
            _cardId = cardId;
            _atmEntity = atmEntity;
        }

        /// <summary>
        /// The bank card whose PIN was typed for this session.
        /// </summary>
        public ulong GetCardId()
        {
            return _cardId;
        }

        /// <summary>
        /// The ATM in use, or null when it despawned.
        /// </summary>
        public Atm? GetAtm()
        {
            return Manager.Exists(_atmEntity) ? Manager.GetComponent<Atm>(_atmEntity) : null;
        }
    }
}
