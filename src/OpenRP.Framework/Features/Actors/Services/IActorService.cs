using OpenRP.Framework.Database.Models;
using SampSharp.Entities.SAMP;
using OpenRP.Framework.Features.Actors.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Actors.Services
{
    public interface IActorService
    {
        List<ActorModel> GetActors();
        void LoadActors();
        List<ServerActor> GetServerActors();

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
