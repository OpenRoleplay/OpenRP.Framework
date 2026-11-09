using OpenRP.Framework.Features.ActorRoleplay.Services;
using OpenRP.Framework.Shared.ServerEvents.Attributes;
using OpenRP.Framework.Shared.ServerEvents.Entities;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;

namespace OpenRP.Framework.Features.ActorRoleplay.Systems
{
    public class ActorRoleplayServerSystem : IServerSystem
    {
        [ServerEvent]
        public void OnPlayerChatMessageSent(OnPlayerChatMessageSentEventArgs args, IActorRoleplayService actorRoleplayService)
        {
            actorRoleplayService.HearPlayerMessage(args);
        }
    }
}
