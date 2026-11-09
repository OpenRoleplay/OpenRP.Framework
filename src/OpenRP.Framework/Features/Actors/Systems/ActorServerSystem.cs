using OpenRP.Framework.Features.Actors.Services;
using OpenRP.Framework.Shared.ServerEvents.Attributes;
using OpenRP.Framework.Shared.ServerEvents.Entities;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;

namespace OpenRP.Framework.Features.Actors.Systems
{
    public class ActorServerSystem : IServerSystem
    {
        /// <summary>
        /// The database puts these actors outside too, but loaded actors would save the old property back over it.
        /// </summary>
        [ServerEvent]
        public void OnPropertyDeleted(OnPropertyDeletedEventArgs args, IActorEditorService actorEditorService)
        {
            actorEditorService.MoveActorsOutsideOf(args.PropertyId);
        }
    }
}
