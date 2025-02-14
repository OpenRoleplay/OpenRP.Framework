using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Database.Models
{
    public class ActorRelationshipModel
    {
        public ulong Id { get; set; }
        public ulong ActorId { get; set; }
        public ulong ActorRelationshipWithActorId { get; set; }
        public string RelationshipDescription { get; set; }

        // Navigational Properties
        public ActorModel ActorRelationshipWithActor { get; set; }
        public ActorModel Actor { get; set; }
    }
}
