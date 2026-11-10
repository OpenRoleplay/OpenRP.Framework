using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Phones.Services
{
    /// <summary>
    /// The /me lines of using a phone, so people nearby see someone dial, answer, hang up or text. They go through
    /// the chat like any /me, so AI actors notice them too; they never go into a call.
    /// </summary>
    public interface IPhoneRoleplayService
    {
        void SendDialAction(Player player);

        void SendAnswerAction(Player player);

        void SendDeclineAction(Player player);

        void SendHangUpAction(Player player);

        /// <summary>
        /// The call ended without the player hanging up: nobody answered, the other side hung up, or it was cut off.
        /// </summary>
        void SendLowerPhoneAction(Player player);

        void SendTextAction(Player player);
    }
}
