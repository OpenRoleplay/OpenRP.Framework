namespace OpenRP.Framework.Features.ActorRoleplay.Entities
{
    public class ActorRoleplayOptions
    {
        /// <summary>
        /// The OpenAI API key. Without it, actors don't reply.
        /// </summary>
        public string? OpenAiApiKey { get; set; }

        /// <summary>
        /// The OpenAI model actors reply with. A cheap, fast model is enough for short roleplay replies.
        /// </summary>
        public string ModelId { get; set; } = "gpt-6-luna";

        /// <summary>
        /// "none", "low", "medium" or "high" for reasoning models; null or empty for models that don't reason
        /// (they reject the setting). "none" skips thinking, which is billed as output, the expensive part.
        /// </summary>
        public string? ReasoningEffort { get; set; } = "none";

        /// <summary>
        /// How many OpenAI requests may run at once across all actors. Other actors wait their turn, which keeps a
        /// crowd under OpenAI's rate limits.
        /// </summary>
        public int MaxConcurrentReplies { get; set; } = 4;
    }
}
