using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.ActorConversations.Helpers;
using OpenRP.Framework.Features.Actors.Services;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Features.Actors.Components;
using Microsoft.Extensions.Options;
using OpenRP.Framework.Features.Discord.Entities;
using OpenRP.Framework.Features.ActorConversations.Entities;

namespace OpenRP.Framework.Features.ActorConversations.Services
{
    public class ActorConversationWithPlayerManager : IActorConversationWithPlayerManager
    {
        private readonly ConcurrentDictionary<string, ActorConversationWithPlayerHelper> _activeConversations;
        private readonly IEntityManager _entityManager;
        private readonly IActorService _actorService;
        private readonly ActorConversationOptions _actorConversationOptions;

        public ActorConversationWithPlayerManager(IOptions<ActorConversationOptions> options, IEntityManager entityManager, IActorService actorService)
        {
            _activeConversations = new ConcurrentDictionary<string, ActorConversationWithPlayerHelper>();
            _entityManager = entityManager;
            _actorService = actorService;
            _actorConversationOptions = options.Value;
        }

        /// <summary>
        /// Creates a new conversation with specified actors.
        /// </summary>
        public ActorConversationWithPlayerHelper CreateConversation(string conversationName, List<ulong> actorIds)
        {
            List<ServerActor> actorList = new List<ServerActor>();
            List<ServerActor> loadedActors = _actorService.GetServerActors();
            foreach (ulong actorId in actorIds)
            {
                ServerActor? serverActor = loadedActors.FirstOrDefault(i => i.GetId() == actorId);
                if (serverActor != null)
                {
                    actorList.Add(serverActor);
                }
            }

            return CreateConversation(conversationName, actorList);
        }
        public ActorConversationWithPlayerHelper CreateConversation(string conversationName, List<ServerActor> actors)
        {
            var conversation = new ActorConversationWithPlayerHelper(_actorConversationOptions.ChatGptApiKey, conversationName, actors, _entityManager);
            _activeConversations.TryAdd(conversationName, conversation);
            return conversation;
        }

        /// <summary>
        /// Attaches a player to a specific conversation.
        /// </summary>
        public async Task AttachPlayerToConversationAsync(string conversationName, Player player)
        {
            if (_activeConversations.TryGetValue(conversationName, out var conversation))
            {
                await conversation.AttachPlayerAsync(player);
            }
        }

        /// <summary>
        /// Detaches a player from a specific conversation.
        /// </summary>
        public async Task DetachPlayerFromConversationAsync(string conversationName, Player player)
        {
            if (_activeConversations.TryGetValue(conversationName, out var conversation))
            {
                await conversation.DetachPlayerAsync(player);
            }
        }
    }
}
