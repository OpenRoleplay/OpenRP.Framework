using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.ActorConversations.Components
{
    public class ActorConversationWithPlayerState : Component
    {
        public string ConversationId { get; set; }
        public int CurrentSequence { get; set; }
    }
}
