using OpenRP.Framework.Features.Phones.Components;
using SampSharp.Entities.SAMP;

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

        /// <summary>
        /// The low battery beep at 10%, 5% or 2%, more urgent the lower it is. The holder may have the phone in a wallet.
        /// </summary>
        void WarnLowBattery(Phone phone, Player holder, double warningLevel);

        /// <summary>
        /// The last beep before the phone switches itself off.
        /// </summary>
        void AnnounceEmptyBattery(Phone phone, Player holder);
    }
}
