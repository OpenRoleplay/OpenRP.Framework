using OpenRP.Framework.Features.EmergencyCalls.Enums;

namespace OpenRP.Framework.Features.EmergencyCalls.Services
{
    /// <summary>
    /// Relays calls to the units. Until radios exist, everyone with the dispatch.receive permission hears it.
    /// </summary>
    public interface IDispatchService
    {
        /// <summary>
        /// Sends dispatch's radio broadcast to every online character with dispatch.receive, and each of them does a
        /// /do about their radio so people nearby notice.
        /// </summary>
        void Broadcast(EmergencyServiceType service, string text);
    }
}
