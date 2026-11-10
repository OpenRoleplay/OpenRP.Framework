using OpenRP.Framework.Features.Phones.Components;

namespace OpenRP.Framework.Features.Phones.Services
{
    /// <summary>
    /// What a phone does to get noticed. When it's on and not silent, its carrier does a /do that people nearby see;
    /// when silent, only the carrier notices; when off, nothing happens.
    /// </summary>
    public interface IPhoneAlertService
    {
        /// <summary>
        /// One ring of an incoming call.
        /// </summary>
        void Ring(Phone phone, uint callerNumber);

        /// <summary>
        /// The beep of a text that just came in.
        /// </summary>
        void BeepForMessage(Phone phone, uint senderNumber);
    }
}
