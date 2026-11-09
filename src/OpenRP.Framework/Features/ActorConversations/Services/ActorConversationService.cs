using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenRP.Framework.Features.ActorConversations.Components;
using OpenRP.Framework.Features.ActorConversations.Entities;
using OpenRP.Framework.Features.ActorConversations.Helpers;
using OpenRP.Framework.Features.ActorRoleplay.Entities;
using OpenRP.Framework.Features.ActorRoleplay.Services;
using OpenRP.Framework.Features.Actors.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.Chat.Helpers;
using OpenRP.Framework.Shared.Chat.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.ActorConversations.Services
{
    /// <summary>
    /// Plays cached conversation lines to viewers, and writes the next turn, one character at a time, only when a
    /// viewer gets close to the end. Runs on the main thread: viewers advance on a timer, and SampSharp brings each
    /// await back to the main thread.
    /// </summary>
    public class ActorConversationService : IActorConversationService
    {
        // What the next speaker remembers of the conversation; enough to follow the story.
        private const int MemoryLineCount = 25;
        // A failed turn waits this long before the next try, so an OpenAI outage doesn't cause a stream of requests.
        private static readonly TimeSpan RetryAfterFailure = TimeSpan.FromSeconds(30);

        private readonly IEntityManager _entityManager;
        private readonly IActorService _actorService;
        private readonly IChatService _chatService;
        private readonly IActorReplyGenerator _replyGenerator;
        private readonly ActorConversationOptions _options;
        private readonly ILogger<ActorConversationService> _logger;

        public ActorConversationService(IEntityManager entityManager, IActorService actorService, IChatService chatService,
            IActorReplyGenerator replyGenerator, IOptions<ActorConversationOptions> options, ILogger<ActorConversationService> logger)
        {
            _entityManager = entityManager;
            _actorService = actorService;
            _chatService = chatService;
            _replyGenerator = replyGenerator;
            _options = options.Value;
            _logger = logger;
        }

        public void CreateConversation(string key, List<ulong> actorIds, string? situation = null)
        {
            // No null constructor arguments: the component is created from this argument list.
            _entityManager.AddComponent<ActorConversation>(EntityId.NewEntityId(), key, actorIds, situation ?? string.Empty);
        }

        public void AttachPlayer(string key, Player player)
        {
            DetachPlayer(player);
            player.AddComponent<ActorConversationViewer>(key);
        }

        public void DetachPlayer(Player player)
        {
            player.DestroyComponents<ActorConversationViewer>();
        }

        public void AdvanceViewers()
        {
            foreach (ActorConversationViewer viewer in _entityManager.GetComponents<ActorConversationViewer>().ToList())
            {
                if (viewer.IsNextLineDue())
                {
                    AdvanceViewer(viewer);
                }
            }
        }

        private void AdvanceViewer(ActorConversationViewer viewer)
        {
            ActorConversation? conversation = GetConversation(viewer.ConversationKey);
            Player? player = viewer.GetComponent<Player>();
            if (conversation == null || player == null)
            {
                return;
            }

            if (viewer.NextLineIndex < conversation.GetLineCount())
            {
                SendNextLine(viewer, player, conversation);
            }
            else if (conversation.IsFull(_options.MaxCachedLines))
            {
                SendLoginReminderOnce(viewer, player);
            }

            GenerateAheadOfViewer(viewer, conversation);
        }

        private ActorConversation? GetConversation(string key)
        {
            return _entityManager.GetComponents<ActorConversation>().FirstOrDefault(conversation => conversation.Key == key);
        }

        private void SendNextLine(ActorConversationViewer viewer, Player player, ActorConversation conversation)
        {
            ActorConversationLine line = conversation.GetLine(viewer.NextLineIndex);
            string name = conversation.GetParticipant(line.ActorId)?.Name ?? "";

            foreach (string part in ChatLineSplitter.Split(line.Text))
            {
                _chatService.SendRoleplayLineToPlayer(player, line.Type, name, part);
            }

            viewer.MoveToNextLine(ChatTypingDelay.Get(line.Text));
        }

        /// <summary>
        /// The conversation is over for this viewer; tells them once, as the old main menu did.
        /// </summary>
        private static void SendLoginReminderOnce(ActorConversationViewer viewer, Player player)
        {
            if (viewer.HasSeenLoginReminder)
            {
                return;
            }

            viewer.MarkLoginReminderSeen();
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "We think you've seen enough of this conversation. Perhaps it's time to create an account or login now? ;)");
        }

        /// <summary>
        /// Writes the next turn when this viewer is close to the end, so the lines are ready before they're needed.
        /// Viewers further behind cost nothing.
        /// </summary>
        private void GenerateAheadOfViewer(ActorConversationViewer viewer, ActorConversation conversation)
        {
            bool isCloseToEnd = viewer.NextLineIndex >= conversation.GetLineCount() - _options.LinesAhead;

            if (isCloseToEnd && !conversation.IsFull(_options.MaxCachedLines) && conversation.CanGenerate() && _replyGenerator.IsConfigured())
            {
                _ = GenerateNextTurnAsync(conversation);
            }
        }

        /// <summary>
        /// Fire-and-forget from the timer, so every error is caught and logged here instead of being lost.
        /// </summary>
        private async Task GenerateNextTurnAsync(ActorConversation conversation)
        {
            conversation.BeginGenerating();
            TimeSpan retryAfter = TimeSpan.Zero;

            try
            {
                if (!TryLoadParticipants(conversation) || await WriteNextTurnAsync(conversation) == 0)
                {
                    // Missing actors or a reply without usable lines would otherwise be retried on the very next tick.
                    retryAfter = RetryAfterFailure;
                }
            }
            catch (Exception exception)
            {
                retryAfter = RetryAfterFailure;
                _logger.LogError(exception, "Couldn't write the next turn of conversation {ConversationKey}.", conversation.Key);
            }
            finally
            {
                if (conversation.IsComponentAlive)
                {
                    conversation.EndGenerating(retryAfter);
                }
            }
        }

        /// <summary>
        /// Lets the next speaker reply to what they remember, adds their lines and picks who speaks after them.
        /// Returns how many lines were added.
        /// </summary>
        private async Task<int> WriteNextTurnAsync(ActorConversation conversation)
        {
            ActorConversationParticipant speaker = conversation.GetParticipant(conversation.NextSpeakerId) ?? conversation.Participants[0];

            List<ActorRoleplayLine> reply = await _replyGenerator.GenerateReplyAsync(speaker.Name, speaker.Prompt,
                GetSpeakerMemory(conversation, speaker), GetSituation(conversation, speaker));

            // The conversation can be gone by the time the model answers.
            if (!conversation.IsComponentAlive || reply.Count == 0)
            {
                return 0;
            }

            conversation.AddLines(reply.Select(line => new ActorConversationLine(speaker.ActorId, line.Type, line.Text)), _options.MaxCachedLines);
            conversation.SetNextSpeaker(ActorConversationTurns.ChooseNextSpeaker(conversation.Participants, conversation.GetLines(), speaker.ActorId));

            _logger.LogInformation("Generated {LineCount} lines for conversation {ConversationKey} ({CachedLineCount} cached).",
                reply.Count, conversation.Key, conversation.GetLineCount());
            return reply.Count;
        }

        /// <summary>
        /// The latest lines as the speaker remembers them: their own as commands, everyone else's as shown in chat.
        /// </summary>
        private static List<ActorMemoryLine> GetSpeakerMemory(ActorConversation conversation, ActorConversationParticipant speaker)
        {
            return conversation.GetLastLines(MemoryLineCount)
                .Select(line => line.ActorId == speaker.ActorId
                    ? new ActorMemoryLine(true, RoleplayCommandLine.ToCommandText(line.Type, line.Text))
                    : new ActorMemoryLine(false, RoleplayCommandLine.FormatAsChat(line.Type, conversation.GetParticipant(line.ActorId)?.Name ?? "", line.Text)))
                .ToList();
        }

        /// <summary>
        /// Where the speaker is and who with, so the conversation keeps going instead of ending in silence.
        /// </summary>
        private static string GetSituation(ActorConversation conversation, ActorConversationParticipant speaker)
        {
            string others = String.Join(" and ", conversation.Participants.Where(participant => participant.ActorId != speaker.ActorId).Select(participant => participant.Name));
            string start = conversation.GetLineCount() == 0 ? " Nobody has said anything yet: start the scene." : "";
            return $"{conversation.Situation} You're together with {others}. Keep the conversation going: talk to them and react to them.{start}".Trim();
        }

        /// <summary>
        /// Fills in the conversation's actors if that hasn't happened yet. False when none of them is loaded.
        /// </summary>
        private bool TryLoadParticipants(ActorConversation conversation)
        {
            if (!conversation.HasParticipants())
            {
                conversation.SetParticipants(GetParticipants(conversation.ActorIds));
            }

            if (conversation.HasParticipants())
            {
                return true;
            }

            _logger.LogWarning("None of the actors of conversation {ConversationKey} are loaded.", conversation.Key);
            return false;
        }

        private List<ActorConversationParticipant> GetParticipants(IReadOnlyList<ulong> actorIds)
        {
            return _actorService.GetServerActors()
                .Where(serverActor => actorIds.Contains(serverActor.GetId()))
                .Select(serverActor => new ActorConversationParticipant(serverActor.GetId(), serverActor.GetName() ?? $"Actor {serverActor.GetId()}", serverActor.GetActorPrompt()))
                .ToList();
        }
    }
}
