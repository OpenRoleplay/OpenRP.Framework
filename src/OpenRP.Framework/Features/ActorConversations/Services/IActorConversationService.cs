using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.ActorConversations.Services
{
    public interface IActorConversationService
    {
        /// <summary>
        /// Creates a conversation between the actors with these ids, under a key that players can be attached to.
        /// <paramref name="situation"/> describes where it takes place, for every speaker.
        /// </summary>
        void CreateConversation(string key, List<ulong> actorIds, string? situation = null);

        /// <summary>
        /// Lets the player watch the conversation from its first line.
        /// </summary>
        void AttachPlayer(string key, Player player);

        /// <summary>
        /// Stops the player watching whatever conversation they're watching.
        /// </summary>
        void DetachPlayer(Player player);

        /// <summary>
        /// Sends each viewer their next line when it's due, and writes the next turn when a viewer gets close to the end.
        /// </summary>
        void AdvanceViewers();
    }
}
