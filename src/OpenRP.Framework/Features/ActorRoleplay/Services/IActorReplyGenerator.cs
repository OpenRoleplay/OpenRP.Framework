using OpenRP.Framework.Features.ActorRoleplay.Entities;

namespace OpenRP.Framework.Features.ActorRoleplay.Services
{
    /// <summary>
    /// Writes one character's next lines. Each request only knows that one character, so it can't speak for anyone
    /// else; actors answering players and actors in scene conversations both use it.
    /// </summary>
    public interface IActorReplyGenerator
    {
        /// <summary>
        /// Whether an API key is set, so replies can be generated at all.
        /// </summary>
        bool IsConfigured();

        /// <summary>
        /// The actor's reply to what it remembers: up to 3 /me, /do and /say lines, or none without an API key.
        /// <paramref name="situation"/> optionally describes where the actor is and what's expected of it.
        /// <paramref name="cacheKey"/> names the conversation, so its requests share one prompt cache.
        /// </summary>
        Task<List<ActorRoleplayLine>> GenerateReplyAsync(string actorName, string actorPrompt, IReadOnlyList<ActorMemoryLine> lines, string? situation = null, string? cacheKey = null);

        /// <summary>
        /// Like <see cref="GenerateReplyAsync"/>, but the actor may also ask to perform one of these actions.
        /// </summary>
        Task<ActorReply> GenerateReplyWithActionsAsync(string actorName, string actorPrompt, IReadOnlyList<ActorMemoryLine> lines, IReadOnlyList<IActorAction> actions, string? cacheKey = null);
    }
}
