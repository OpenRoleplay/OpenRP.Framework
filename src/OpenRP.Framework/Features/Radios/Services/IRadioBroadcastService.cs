using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Radios.Services
{
    /// <summary>
    /// Sends a transmission to every radio on the frequency. Holders see it in chat; people near a radio that plays
    /// out loud overhear it.
    /// </summary>
    public interface IRadioBroadcastService
    {
        /// <summary>
        /// The player says the text out loud and sends it over the radio they use. Returns why it couldn't, or null.
        /// </summary>
        string? TransmitFromPlayer(Player player, string text);

        /// <summary>
        /// Sends a transmission that comes from the server, such as 911 dispatch. It may talk on any frequency.
        /// </summary>
        void TransmitFromSystem(uint frequency, string senderName, string text);
    }
}
