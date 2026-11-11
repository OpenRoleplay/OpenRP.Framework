using OpenRP.Framework.Features.EmergencyCalls.Enums;

namespace OpenRP.Framework.Features.EmergencyCalls.Services
{
    /// <summary>
    /// Relays calls to the units over the dispatch frequency (911 by default).
    /// </summary>
    public interface IDispatchService
    {
        /// <summary>
        /// Sends dispatch's broadcast to every radio on the dispatch frequency.
        /// </summary>
        void Broadcast(EmergencyServiceType service, string text);

        /// <summary>
        /// Creates the dispatch frequency as protected, with no factions, unless it exists: anyone with a radio can
        /// listen, and admins add the factions that may talk.
        /// </summary>
        void CreateDispatchFrequencyIfMissing();
    }
}
