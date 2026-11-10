using OpenRP.Framework.Features.Phones.Components;
using OpenRP.Framework.Features.Phones.Entities;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Phones.Services
{
    /// <summary>
    /// A server number's view of one call. It wraps the call component, so it stops working once the call ended.
    /// </summary>
    internal sealed class ServerPhoneLine : IServerPhoneLine
    {
        private readonly PhoneCall _call;
        private readonly IPhoneCallService _phoneCallService;
        private readonly IPhoneService _phoneService;
        private readonly IEntityManager _entityManager;
        private readonly EntityId _callId;

        public ServerPhoneLine(PhoneCall call, IPhoneCallService phoneCallService, IPhoneService phoneService, IEntityManager entityManager)
        {
            _call = call;
            _entityManager = entityManager;
            _phoneCallService = phoneCallService;
            _phoneService = phoneService;
            _callId = call.Entity;
        }

        public EntityId GetCallId()
        {
            return _callId;
        }

        public bool IsActive()
        {
            return _call.IsComponentAlive;
        }

        public Player? GetCaller()
        {
            return IsActive() ? _entityManager.GetComponent<Player>(_call.GetCallerPlayer()) : null;
        }

        public uint GetCallerNumber()
        {
            return IsActive() ? _phoneService.GetPhone(_call.GetCallerPhoneId())?.GetNumber() ?? 0 : 0;
        }

        public void Say(string speakerName, string text)
        {
            if (IsActive())
            {
                _phoneCallService.SayToCaller(_call, speakerName, text);
            }
        }

        public void HangUp()
        {
            if (IsActive())
            {
                _phoneCallService.EndCall(_call);
            }
        }
    }
}
