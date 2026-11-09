using OpenRP.Framework.Features.ActorRoleplay.Entities;

namespace OpenRP.Framework.Features.ActorRoleplay.Services
{
    public interface IActorReplyGenerator
    {
        /// <summary>
        /// Whether an API key is set, so replies can be generated at all.
        /// </summary>
        bool IsConfigured();

        /// <summary>
        /// Asks the model for the actor's reply to what it remembers. Returns the raw reply text, or null without one.
        /// </summary>
        Task<string?> GenerateReplyAsync(string actorName, string actorPrompt, IReadOnlyList<ActorMemoryLine> lines);
    }
}
