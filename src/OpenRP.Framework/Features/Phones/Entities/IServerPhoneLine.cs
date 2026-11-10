using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Phones.Entities
{
    /// <summary>
    /// One call to a server number, as the server number sees it: who's calling, and how to talk back or hang up.
    /// </summary>
    public interface IServerPhoneLine
    {
        /// <summary>
        /// Stays the same for the whole call, so a server number can keep its own state per call.
        /// </summary>
        EntityId GetCallId();

        /// <summary>
        /// Whether the call is still going. After an await, check this before talking.
        /// </summary>
        bool IsActive();

        /// <summary>
        /// The calling player, or null once the call ended.
        /// </summary>
        Player? GetCaller();

        uint GetCallerNumber();

        /// <summary>
        /// Says something to the caller, shown as "(Phone) speakerName: text".
        /// </summary>
        void Say(string speakerName, string text);

        void HangUp();
    }
}
