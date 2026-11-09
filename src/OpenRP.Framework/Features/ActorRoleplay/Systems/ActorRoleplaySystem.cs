using OpenRP.Framework.Features.ActorRoleplay.Services;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.ActorRoleplay.Systems
{
    public class ActorRoleplaySystem : ISystem
    {
        // Replies wait for a 2 second pause, so checking twice a second is precise enough.
        private const int ReplyCheckIntervalInMilliseconds = 500;
        private const int ForgetCheckIntervalInMilliseconds = 60000;

        [Timer(ReplyCheckIntervalInMilliseconds)]
        public void ReplyToQuietActors(IActorRoleplayService actorRoleplayService)
        {
            actorRoleplayService.ReplyToQuietActors();
        }

        [Timer(ForgetCheckIntervalInMilliseconds)]
        public void ForgetIdleActors(IActorRoleplayService actorRoleplayService)
        {
            actorRoleplayService.ForgetIdleActors();
        }
    }
}
