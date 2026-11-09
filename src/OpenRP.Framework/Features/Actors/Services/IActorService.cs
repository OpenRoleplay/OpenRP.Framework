using SampSharp.Entities.SAMP;
using OpenRP.Framework.Features.Actors.Components;

namespace OpenRP.Framework.Features.Actors.Services
{
    public interface IActorService
    {
        List<ServerActor> GetServerActors();

        /// <summary>
        /// The virtual world the actor stands in: its main menu scene's, the property's it's inside, or 0 outside.
        /// </summary>
        int GetActorVirtualWorld(ServerActor serverActor);

        /// <summary>
        /// (Re)creates the actor's game actor and name label from its current data, replacing the old ones.
        /// </summary>
        void CreateActorInWorld(ServerActor serverActor);

        /// <summary>
        /// Removes the actor's game actor and name label from the world.
        /// </summary>
        void DestroyActorInWorld(ServerActor serverActor);

        /// <summary>
        /// Sends the actor's position and animation again shortly, after it streamed in for a player and the mapping under it may not have loaded yet.
        /// </summary>
        void ScheduleSpawnPoseRefresh(Actor actor);

        /// <summary>
        /// Puts every actor whose refresh is due back on its spot.
        /// </summary>
        void RefreshDueSpawnPoses();
    }
}
