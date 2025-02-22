using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Actors.Entities
{
    public static class ActorEntities
    {
        [EntityType]
        public static readonly Guid ServerActorType = new Guid("B7ADDC12-E372-4C1F-8F61-F678B1AB5241");

        public static EntityId GetServerActorId(int serverActorId)
        {
            return new EntityId(ServerActorType, serverActorId);
        }
    }
}
