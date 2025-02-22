using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Actors.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.ActorConversations.Helpers
{
    public class ActorConversationHistory
    {
        public int SequenceNumber { get; set; }
        public ServerActor FromActor { get; set; }
        public DateTime Timestamp { get; set; }
        public ActorProcessedCommand Message { get; set; }

    }
}
