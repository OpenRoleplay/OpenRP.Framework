using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Shared.BaseManager.Helpers;

namespace OpenRP.Framework.Features.Actors.Services
{
    public interface IActorManager : IBaseManager
    {
        /// <summary>
        /// Inserts the actor right away and loads it, so it stands in the world immediately.
        /// </summary>
        ServerActor CreateAndLoad(ActorModel model);
    }
}
