using OpenRP.Framework.Features.Actors.Services;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Actors.Systems
{
    public class ActorSystem : ISystem
    {
        [Event]
        public void OnGameModeInit(IActorService actorService)
        {
            actorService.LoadActors();
        }
    }
}
