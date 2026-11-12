using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Actors.Entities;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Actors.Services
{
    /// <summary>
    /// What actors are allowed to do. An actor has no permissions until an admin gives them in /editactors.
    /// </summary>
    public interface IActorPermissionService
    {
        /// <summary>
        /// Every permission features offer, in the order they were registered.
        /// </summary>
        IReadOnlyList<ActorPermissionDefinition> GetDefinitions();

        bool HasPermission(ServerActor actor, string permission);

        /// <summary>
        /// Gives or takes the permission. Saved with the next actor sync.
        /// </summary>
        void SetPermission(ServerActor actor, string permission, bool isGranted);
    }
}
