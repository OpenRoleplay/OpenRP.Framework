using OpenRP.Framework.Features.Phones.Components;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Phones.Services
{
    /// <summary>
    /// Calls in progress: dialing, ringing, answering, talking and hanging up. Calls only reach phones carried on this
    /// server. Methods that can fail return the reason, or null.
    /// </summary>
    public interface IPhoneCallService
    {
        /// <summary>
        /// Calls the number from the player's phone. What the caller hears (ringing, busy, not in service) comes as
        /// phone lines; the error is only for things like a phone that's off.
        /// </summary>
        string? Dial(Player caller, Phone phone, uint number);

        /// <summary>
        /// Answers the call ringing on a phone the player carries.
        /// </summary>
        string? Answer(Player player);

        /// <summary>
        /// Ends the player's call, or declines the one ringing on their phone.
        /// </summary>
        string? HangUp(Player player);

        /// <summary>
        /// The call the player is in, ringing or connected, as caller or as the one called.
        /// </summary>
        PhoneCall? GetCallOf(Player player);

        bool IsPhoneInCall(Phone phone);

        /// <summary>
        /// Passes what the player said nearby on to the other side of their connected call.
        /// </summary>
        void PassOnSpeech(Player speaker, string text);

        /// <summary>
        /// Rings, gives up on unanswered calls, and ends calls whose phone was lost or turned off. Called every second.
        /// </summary>
        void ProcessCalls();

        /// <summary>
        /// Ends the calls of a player who is leaving.
        /// </summary>
        void EndCallsOf(Player player);

        /// <summary>
        /// Says something to the caller of a call, shown as "(Phone) speakerName: text". For server numbers.
        /// </summary>
        void SayToCaller(PhoneCall call, string speakerName, string text);

        /// <summary>
        /// Ends the call for both sides.
        /// </summary>
        void EndCall(PhoneCall call);
    }
}
