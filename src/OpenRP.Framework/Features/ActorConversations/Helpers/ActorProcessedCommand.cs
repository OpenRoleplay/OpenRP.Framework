using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Actors.Components;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.ActorConversations.Helpers
{
    public class ActorProcessedCommand
    {
        public string OriginalCommand { get; set; }
        public ServerActor Actor { get; set; }
        public bool SendChatMessage { get; set; }
        public string Message { get; set; }
        public Color ChatColor { get; set; }
        public int TypingTime { get; set; }
    }
}
