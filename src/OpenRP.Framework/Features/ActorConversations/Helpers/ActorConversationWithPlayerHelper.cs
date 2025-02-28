using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.ActorConversations.Components;
using OpenRP.Framework.Shared.Chat.Enums;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatGPT.Net;
using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Shared.Chat.Extensions;

namespace OpenRP.Framework.Features.ActorConversations.Helpers
{
    public class ActorConversationWithPlayerHelper
    {
        private string _conversationId;
        private List<ServerActor> _conversationActorsParticipating;
        private List<ActorConversationHistory> _conversationHistory;
        private readonly SemaphoreSlim _conversationLock;
        private bool _conversationIsActive;

        private ChatGpt _chatGpt;

        private IEntityManager _entityManager;

        public ActorConversationWithPlayerHelper(string apiKey, string conversationId, List<ServerActor> conversationActorsParticipating, IEntityManager entityManager)
        {
            _conversationId = conversationId;
            _conversationActorsParticipating = conversationActorsParticipating;
            _conversationHistory = new List<ActorConversationHistory>();
            _conversationLock = new SemaphoreSlim(1, 1);
            _conversationIsActive = false;

            _chatGpt = new ChatGpt(apiKey);

            ActorConversationWithPlayerPromptBuilder actorPromptBuilder = new ActorConversationWithPlayerPromptBuilder(_conversationActorsParticipating);
            _chatGpt.SetConversationSystemMessage(_conversationId, actorPromptBuilder.ToString());

            _entityManager = entityManager;
        }

        /// <summary>
        /// Attaches a player to the conversation.
        /// </summary>
        public async Task AttachPlayerAsync(Player player)
        {
            await _conversationLock.WaitAsync();
            try
            {
                // Check if player is already attached
                ActorConversationWithPlayerState state = player.GetComponent<ActorConversationWithPlayerState>();

                if (state == null)
                {
                    state = player.AddComponent<ActorConversationWithPlayerState>();
                    state.ConversationId = _conversationId;
                    state.CurrentSequence = 0;

                    // Send the entire history to the player
                    foreach (var historyEntry in _conversationHistory.OrderBy(i => i.SequenceNumber))
                    {
                        state = player.GetComponent<ActorConversationWithPlayerState>();

                        if (state == null)
                        {
                            break;
                        }

                        if (historyEntry.Message.SendChatMessage)
                        {
                            player.SendClientMessage(historyEntry.Message.ChatColor, historyEntry.Message.Message);
                        }
                        state.CurrentSequence = historyEntry.SequenceNumber;

                        await Task.Delay(TimeSpan.FromMilliseconds(historyEntry.Message.TypingTime));
                    }
                }

                // Activate conversation if it was inactive
                if (!_conversationIsActive)
                {
                    _conversationIsActive = true;
                    _ = Task.Run(() => RunConversationLoopAsync());
                }
            }
            finally
            {
                _conversationLock.Release();
            }
        }


        /// <summary>
        /// Detaches a player from the conversation.
        /// </summary>
        public async Task DetachPlayerAsync(Player player)
        {
            ActorConversationWithPlayerState state = player.GetComponent<ActorConversationWithPlayerState>();

            if (state != null)
            {
                player.DestroyComponents<ActorConversationWithPlayerState>();
            }

            await _conversationLock.WaitAsync();
            try
            {
                // Deactivate conversation if no players are attached
                List<ActorConversationWithPlayerState> currentlyAttachedPlayers = GetCurrentlyAttachedPlayerStates();
                int attachedPlayerCount = currentlyAttachedPlayers.Count;
                if (attachedPlayerCount == 0)
                {
                    _conversationIsActive = false;
                }
            }
            finally
            {
                _conversationLock.Release();
            }
        }

        private List<ActorConversationWithPlayerState> GetCurrentlyAttachedPlayerStates()
        {
            return _entityManager.GetComponents<ActorConversationWithPlayerState>().Where(i => i.ConversationId == _conversationId).ToList();
        }

        /// <summary>
        /// Runs the conversation loop, fetching new exchanges as needed.
        /// </summary>
        private async Task RunConversationLoopAsync()
        {
            Random random = new Random();
            while (_conversationIsActive)
            {
                await _conversationLock.WaitAsync();
                try
                {
                    // Determine if at least one player has caught up
                    bool canGenerateNewExchange = _entityManager.GetComponents<ActorConversationWithPlayerState>().Any(ps => ps.CurrentSequence == _conversationHistory.Count);

                    if (canGenerateNewExchange == true && _conversationHistory.Count >= 100)
                    {
                        canGenerateNewExchange = false;

                        List<ActorConversationWithPlayerState> currentlyAttachedPlayers = GetCurrentlyAttachedPlayerStates();
                        foreach (var playerState in currentlyAttachedPlayers)
                        {
                            Player player = null;
                            if (playerState.Entity.IsOfType(SampEntities.PlayerType))
                            {
                                player = _entityManager.GetComponent<Player>(playerState.Entity);
                            }

                            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "We think you've seen enough of this conversation. Perhaps it's time to create an account or login now? ;)");
                        }
                    }

                    if (canGenerateNewExchange)
                    {
                        // Fetch the last few exchanges to provide context
                        string context = GenerateContext();

                        // Request a new exchange from ChatGPT
                        List<string> newExchange = await GenerateConversationAsync(context);

                        if (newExchange != null)
                        {
                            ActorCommandProcessor actorCommandProcessor = new ActorCommandProcessor(newExchange, _conversationActorsParticipating, _conversationHistory);
                            List<ActorProcessedCommand> processedCommands = actorCommandProcessor.Process();

                            foreach (ActorProcessedCommand processedCommand in processedCommands)
                            {
                                ActorConversationHistory historyEntry = new ActorConversationHistory
                                {
                                    SequenceNumber = _conversationHistory.Count,
                                    FromActor = processedCommand.Actor,
                                    Message = processedCommand,
                                    Timestamp = DateTime.Now.AddYears(-33)
                                };
                                _conversationHistory.Add(historyEntry);

                                // Notify only players who have caught up
                                List<ActorConversationWithPlayerState> currentlyAttachedPlayers = GetCurrentlyAttachedPlayerStates();
                                foreach (var playerState in currentlyAttachedPlayers)
                                {
                                    Player player = null;
                                    if (playerState.Entity.IsOfType(SampEntities.PlayerType))
                                    {
                                        player = _entityManager.GetComponent<Player>(playerState.Entity);
                                    }

                                    if (player != null)
                                    {
                                        ActorConversationWithPlayerState state = player.GetComponent<ActorConversationWithPlayerState>();

                                        if (state == null)
                                        {
                                            continue;
                                        }

                                        if (playerState.CurrentSequence == _conversationHistory.Count - 1)
                                        {
                                            // Send the new exchange to the player
                                            if (processedCommand.SendChatMessage)
                                            {
                                                player.SendClientMessage(processedCommand.ChatColor, processedCommand.Message);
                                                await Task.Delay(TimeSpan.FromMilliseconds(processedCommand.TypingTime));
                                            }
                                            playerState.CurrentSequence = _conversationHistory.Count;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // If ChatGPT returns null, stop the conversation
                            _conversationIsActive = false;
                        }
                    }
                }
                finally
                {
                    _conversationLock.Release();
                }

                // Wait for a defined interval before checking again
                await Task.Delay(TimeSpan.FromSeconds(3));
            }
        }

        /// <summary>
        /// Generates the next conversation exchange based on context.
        /// </summary>
        public async Task<List<string>> GenerateConversationAsync(string context)
        {
            string response = String.Empty, action = String.Empty;
            if (_conversationHistory.Count == 0)
            {
                action = "Start";
            }
            else
            {
                action = "Continue";
            }

            string history = String.Empty;
            if (_conversationHistory.Count > 0)
            {
                history = $"\n\nHistory (top = oldest, bottom = latest):\n{context}";
            }

            response = await _chatGpt.Ask($"** {action} the conversation with 10 lines of /me, /do and or /say. Use /switchactor the line before a command to switch between actors. No speech on /me lines or actions on /say lines! **{history}", _conversationId);

            ActorCommandParser actorCommandParser = new ActorCommandParser(response);
            List<string> commandList = actorCommandParser.Parse();

            return commandList;
        }

        /// <summary>
        /// Generates the context string for ChatGPT based on recent conversation history.
        /// </summary>
        private string GenerateContext()
        {
            // Concatenate the last few exchanges to provide context
            var recentHistory = _conversationHistory.OrderByDescending(i => i.Timestamp).Take(25).OrderBy(i => i.Timestamp);

            return "- " + String.Join("\n- ", recentHistory.Select(h => $"[{h.Timestamp.ToString()}] {h.Message.OriginalCommand}"));
        }
    }

}
