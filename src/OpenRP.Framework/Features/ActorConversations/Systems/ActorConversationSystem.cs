using OpenRP.Framework.Features.ActorConversations.Services;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.ActorConversations.Systems
{
    public class ActorConversationSystem : ISystem
    {
        // Lines are a second or more apart, so a quarter second is precise enough.
        private const int AdvanceIntervalInMilliseconds = 250;

        [Timer(AdvanceIntervalInMilliseconds)]
        public void AdvanceViewers(IActorConversationService actorConversationService)
        {
            actorConversationService.AdvanceViewers();
        }
    }
}
