using SampSharp.Entities;

namespace OpenRP.Framework.Features.ActorRoleplay.Components
{
    /// <summary>
    /// Marks an AI actor that heard something it hasn't replied to yet. The reply timer only looks at these, so its cost
    /// depends on the actors being talked to, not on how many actors exist.
    /// </summary>
    public class ActorAwaitingReply : Component
    {
    }
}
