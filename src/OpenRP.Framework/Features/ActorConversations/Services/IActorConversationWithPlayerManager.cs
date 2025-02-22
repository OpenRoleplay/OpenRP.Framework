using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.ActorConversations.Helpers;
using OpenRP.Framework.Features.Actors.Components;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.ActorConversations.Services
{
    public interface IActorConversationWithPlayerManager
    {
        ActorConversationWithPlayerHelper CreateConversation(string conversationName, List<ServerActor> actors);
        ActorConversationWithPlayerHelper CreateConversation(string conversationName, List<ulong> actorIds);
        Task AttachPlayerToConversationAsync(string conversationName, Player player);
        Task DetachPlayerFromConversationAsync(string conversationName, Player player);
    }
}
