using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;

namespace OpenRP.Framework.Features.ActorRoleplay.Services
{
    public interface IActorRoleplayService
    {
        /// <summary>
        /// Lets every AI actor that could see or hear this /me, /do or /say remember it, and marks it as awaiting a reply.
        /// </summary>
        void HearPlayerMessage(OnPlayerChatMessageSentEventArgs args);

        /// <summary>
        /// Starts a reply for each actor awaiting one that hasn't heard anything for a moment, while request slots are free.
        /// </summary>
        void ReplyToQuietActors();

        /// <summary>
        /// Removes the memory of actors that haven't heard anything for a long time, so the next visitor starts fresh.
        /// </summary>
        void ForgetIdleActors();
    }
}
