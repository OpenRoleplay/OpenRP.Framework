using OpenRP.Framework.Database.Models;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using SampSharp.Streamer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Actors.Components
{
    public class ServerActor : Component
    {
        private readonly ActorModel _actorModel;
        public ServerActor(ActorModel actorModel) 
        {
            _actorModel = actorModel;
        }

        public Actor GetActor()
        {
            return this.GetComponentInChildren<Actor>();
        }

        public DynamicTextLabel GetNameTag()
        {
            return this.GetComponentInChildren<DynamicTextLabel>();
        }

        public string GetName()
        {
            if (_actorModel.ActorCharacter != null)
            {
                return String.Format("{0} {1}", _actorModel.ActorCharacter.FirstName, _actorModel.ActorCharacter.LastName);
            }
            return null;
        }
    }
}
