namespace OpenRP.Framework.Features.ActorConversations.Entities
{
    /// <summary>
    /// Conversation settings. The model and API key are ActorRoleplay's, because every turn is written by its per-actor generator.
    /// </summary>
    public class ActorConversationOptions
    {
        /// <summary>
        /// How many lines a conversation gets at most. A viewer who reaches the end is told it's time to log in, and
        /// nothing more is requested, which caps what a conversation can cost.
        /// </summary>
        public int MaxCachedLines { get; set; } = 100;

        /// <summary>
        /// The next turn is written once a viewer is this many lines from the end, so nobody waits for a request.
        /// </summary>
        public int LinesAhead { get; set; } = 5;
    }
}
