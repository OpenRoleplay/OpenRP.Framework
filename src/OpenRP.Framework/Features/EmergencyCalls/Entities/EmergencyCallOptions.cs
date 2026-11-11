namespace OpenRP.Framework.Features.EmergencyCalls.Entities
{
    public class EmergencyCallOptions
    {
        /// <summary>
        /// The OpenAI API key. Without it, 911 callers fill in a short form instead of talking to an AI dispatcher.
        /// </summary>
        public string? OpenAiApiKey { get; set; }

        /// <summary>
        /// The OpenAI model the dispatcher talks with.
        /// </summary>
        public string ModelId { get; set; } = "gpt-6-luna";

        /// <summary>
        /// "none", "low", "medium" or "high" for reasoning models; null or empty for models that don't reason.
        /// "none" skips thinking, which is billed as output, the expensive part.
        /// </summary>
        public string? ReasoningEffort { get; set; } = "none";

        /// <summary>
        /// The radio frequency dispatch broadcasts on, for police, fire and EMS alike. It's created at startup as a
        /// protected frequency with no factions: anyone with a radio listens, and admins add the factions that may talk.
        /// </summary>
        public uint DispatchFrequency { get; set; } = 911;
    }
}
