using OpenRP.Framework.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Database.Models
{
    public class ActorRelationshipModel : BaseModel
    {
        public ulong ActorId { get; set; }
        public ulong ActorRelationshipWithActorId { get; set; }
        public string RelationshipDescription { get; set; }

        // Navigational Properties
        public ActorModel ActorRelationshipWithActor { get; set; }
        public ActorModel Actor { get; set; }
    }
}
