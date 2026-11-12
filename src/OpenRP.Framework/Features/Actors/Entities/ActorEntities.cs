using OpenRP.Framework.Shared.BaseManager.Entities;
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
        public static readonly Guid ServerActorType = new Guid("B7ADDC12-E372-4C1F-8F61-F678B1AB5241");

        public static readonly Guid ActorPermissionType = new Guid("4F1C8A2D-9B37-4E65-A0D2-6C5E7B183F94");

        public static EntityId GetServerActorId(int serverActorId)
        {
            return RowEntityIds.Get(ServerActorType, serverActorId);
        }
    }
}
