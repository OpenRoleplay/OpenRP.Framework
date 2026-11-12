using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenRP.Framework.Features.ActorRoleplay.Components;
using OpenRP.Framework.Features.ActorRoleplay.Entities;
using OpenRP.Framework.Features.ActorRoleplay.Helpers;
using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Actors.Services;
using OpenRP.Framework.Shared.AI.Helpers;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Helpers;
using OpenRP.Framework.Shared.Chat.Services;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.ActorRoleplay.Services
{
    /// <summary>
    /// AI actors hear /me, /do and /say around them and reply once the players pause. Runs on the main thread: replies
    /// are started from a timer, and SampSharp brings each await back to the main thread.
    /// </summary>
    public class ActorRoleplayService : IActorRoleplayService
    {
        // Players often send a /me and a /say right after each other; waiting for a pause answers both at once.
        private static readonly TimeSpan QuietTimeBeforeForgetting = TimeSpan.FromMinutes(15);
        // An action is only for someone who spoke to the actor lately and still stands near it.
        private static readonly TimeSpan CustomerMemory = TimeSpan.FromMinutes(5);
        private const float CustomerRange = 10.0f;

        private readonly IEntityManager _entityManager;
        private readonly IChatService _chatService;
        private readonly IActorReplyGenerator _replyGenerator;
        private readonly ActorRoleplayOptions _options;
        private readonly ILogger<ActorRoleplayService> _logger;
        private readonly List<IActorAction> _actions;
        private readonly IActorPermissionService _actorPermissionService;

        // Only touched on the main thread, so a plain counter is enough.
        private int _repliesInProgress;

        public ActorRoleplayService(IEntityManager entityManager, IChatService chatService, IActorReplyGenerator replyGenerator,
            IOptions<ActorRoleplayOptions> options, ILogger<ActorRoleplayService> logger, IEnumerable<IActorAction> actions, IActorPermissionService actorPermissionService)
        {
            _actions = actions.ToList();
            _actorPermissionService = actorPermissionService;
            _entityManager = entityManager;
            _chatService = chatService;
            _replyGenerator = replyGenerator;
            _options = options.Value;
            _logger = logger;
        }

        public void HearPlayerMessage(OnPlayerChatMessageSentEventArgs args)
        {
            if (!_replyGenerator.IsConfigured() || !IsRoleplayMessageType(args.Type))
            {
                return;
            }

            string heardLine = FormatHeardLine(args);

            foreach (ServerActor serverActor in _entityManager.GetComponents<ServerActor>())
            {
                if (CanActorHear(serverActor, args))
                {
                    RememberHeardLine(serverActor, heardLine, args);
                }
            }
        }

        public void ReplyToQuietActors()
        {
            foreach (ActorAwaitingReply awaitingReply in _entityManager.GetComponents<ActorAwaitingReply>().ToList())
            {
                if (_repliesInProgress >= _options.MaxConcurrentReplies)
                {
                    return;
                }

                TryBeginReply(awaitingReply);
            }
        }

        public void ForgetIdleActors()
        {
            foreach (ActorRoleplayMemory memory in _entityManager.GetComponents<ActorRoleplayMemory>().ToList())
            {
                if (!memory.IsReplying() && memory.IsQuietFor(QuietTimeBeforeForgetting) && memory.GetComponent<ActorAwaitingReply>() == null)
                {
                    memory.Destroy();
                }
            }
        }

        private static bool IsRoleplayMessageType(PlayerChatMessageType type)
        {
            return type == PlayerChatMessageType.ME || type == PlayerChatMessageType.DO || type == PlayerChatMessageType.TALK;
        }

        /// <summary>
        /// The line as nearby players saw it in chat.
        /// </summary>
        private static string FormatHeardLine(OnPlayerChatMessageSentEventArgs args)
        {
            return RoleplayCommandLine.FormatAsChat(args.Type, args.CharacterName, args.Text, args.Accent);
        }

        /// <summary>
        /// Whether this is an AI actor in the game world that could see or hear the message. The cheapest checks come
        /// first, because this runs for every actor on every message.
        /// </summary>
        private static bool CanActorHear(ServerActor serverActor, OnPlayerChatMessageSentEventArgs args)
        {
            if (!serverActor.HasActorPrompt() || serverActor.IsInMainMenuScene() || serverActor.GetName() == null)
            {
                return false;
            }

            Actor? actor = serverActor.GetActor();
            return actor != null
                && actor.VirtualWorld == args.VirtualWorld
                && Vector3.Distance(actor.Position, args.Position) <= args.Range;
        }

        private static void RememberHeardLine(ServerActor serverActor, string heardLine, OnPlayerChatMessageSentEventArgs args)
        {
            ActorRoleplayMemory memory = serverActor.GetComponent<ActorRoleplayMemory>() ?? serverActor.AddComponent<ActorRoleplayMemory>();
            memory.AddHeardLine(heardLine);
            if (args.Player != null)
            {
                memory.AddSpeaker(args.Player.Entity, args.CharacterName);
            }

            if (serverActor.GetComponent<ActorAwaitingReply>() == null)
            {
                serverActor.AddComponent<ActorAwaitingReply>();
            }
        }

        private void TryBeginReply(ActorAwaitingReply awaitingReply)
        {
            ServerActor? serverActor = awaitingReply.GetComponent<ServerActor>();
            ActorRoleplayMemory? memory = awaitingReply.GetComponent<ActorRoleplayMemory>();

            if (serverActor == null || memory == null)
            {
                awaitingReply.Destroy();
                return;
            }

            if (memory.IsReplying() || !memory.IsQuietFor(AiReplyTiming.QuietTimeBeforeReply))
            {
                return;
            }

            awaitingReply.Destroy();
            _ = ReplyAsync(serverActor, memory);
        }

        /// <summary>
        /// Fire-and-forget from the timer, so every error is caught and logged here instead of being lost.
        /// </summary>
        private async Task ReplyAsync(ServerActor serverActor, ActorRoleplayMemory memory)
        {
            string actorName = serverActor.GetName();
            memory.BeginReply();
            _repliesInProgress++;

            try
            {
                List<IActorAction> actions = GetAvailableActions(serverActor);
                ActorReply reply = await _replyGenerator.GenerateReplyWithActionsAsync(actorName, serverActor.GetActorPrompt(), memory.GetLines(), actions,
                    cacheKey: $"actor:{serverActor.GetId()}");

                // The actor or its memory can be gone by the time the model answers.
                if (serverActor.IsComponentAlive && memory.IsComponentAlive)
                {
                    await SendReplyLinesAsync(serverActor, memory, reply.Lines);
                }
                if (serverActor.IsComponentAlive && memory.IsComponentAlive)
                {
                    PerformActions(serverActor, memory, reply.Actions);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Actor {ActorName} couldn't reply.", actorName);
            }
            finally
            {
                _repliesInProgress--;
                if (memory.IsComponentAlive)
                {
                    memory.EndReply();
                }
            }
        }

        /// <summary>
        /// The actions this actor has the permission for and can do where it stands. Only these are offered to the model.
        /// </summary>
        private List<IActorAction> GetAvailableActions(ServerActor serverActor)
        {
            return _actions
                .Where(action => _actorPermissionService.HasPermission(serverActor, action.RequiredPermission) && action.IsAvailableFor(serverActor))
                .ToList();
        }

        /// <summary>
        /// Performs what the model asked for, after checking again that the actor may, and only for a player it heard
        /// lately who stands near it. The model can't reach anyone else, whatever name it writes.
        /// </summary>
        private void PerformActions(ServerActor serverActor, ActorRoleplayMemory memory, List<ActorActionRequest> requests)
        {
            foreach (ActorActionRequest request in requests)
            {
                IActorAction? action = GetAvailableActions(serverActor).FirstOrDefault(available => available.Name == request.Name);
                (Player? customer, string? detail) = FindCustomer(serverActor, memory, request.Rest);
                if (action == null || customer == null)
                {
                    _logger.LogInformation("Actor {ActorName} asked for '{Action}', which it can't do or has nobody to do it for.", serverActor.GetName(), request.GetCommandText());
                    continue;
                }

                memory.AddOwnLine(request.GetCommandText());
                action.Perform(serverActor, customer, detail);
            }
        }

        /// <summary>
        /// The recent speaker whose name the text starts with, and what follows the name as the detail. With only one
        /// speaker nearby, that one, even when the model got the name wrong.
        /// </summary>
        private (Player? Customer, string? Detail) FindCustomer(ServerActor serverActor, ActorRoleplayMemory memory, string text)
        {
            List<(ActorSpeaker Speaker, Player Player)> nearby = memory.GetRecentSpeakers(CustomerMemory)
                .Select(speaker => (Speaker: speaker, Player: _entityManager.GetComponent<Player>(speaker.Player)))
                .Where(pair => pair.Player != null && IsNearActor(serverActor, pair.Player))
                .Select(pair => (pair.Speaker, pair.Player!))
                .ToList();

            string normalizedText = text.Replace('_', ' ');
            foreach ((ActorSpeaker speaker, Player player) in nearby)
            {
                if (normalizedText.StartsWith(speaker.Name, StringComparison.OrdinalIgnoreCase))
                {
                    string detail = normalizedText[speaker.Name.Length..].Trim();
                    return (player, detail.Length == 0 ? null : detail);
                }
            }

            return nearby.Count == 1 ? (nearby[0].Player, GetLastWord(normalizedText)) : (null, null);
        }

        private static bool IsNearActor(ServerActor serverActor, Player player)
        {
            Actor? actor = serverActor.GetActor();
            return actor != null && player.IsComponentAlive && actor.VirtualWorld == player.VirtualWorld
                && Vector3.Distance(actor.Position, player.Position) <= CustomerRange;
        }

        /// <summary>
        /// When the name didn't match, a trailing number may still be the detail, such as an amount.
        /// </summary>
        private static string? GetLastWord(string text)
        {
            string lastWord = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? string.Empty;
            return lastWord.Any(char.IsDigit) ? lastWord : null;
        }

        private async Task SendReplyLinesAsync(ServerActor serverActor, ActorRoleplayMemory memory, List<ActorRoleplayLine> lines)
        {
            for (int index = 0; index < lines.Count; index++)
            {
                if (!serverActor.IsComponentAlive || !memory.IsComponentAlive)
                {
                    return;
                }

                memory.AddOwnLine(lines[index].GetCommandText());
                await SendReplyLineAsync(serverActor, lines[index], isLastLine: index == lines.Count - 1);
            }
        }

        /// <summary>
        /// Sends one line, split into parts that fit in chat, pausing between parts as if the actor were typing.
        /// </summary>
        private async Task SendReplyLineAsync(ServerActor serverActor, ActorRoleplayLine line, bool isLastLine)
        {
            List<string> parts = ChatLineSplitter.Split(line.Text);

            for (int index = 0; index < parts.Count; index++)
            {
                if (!serverActor.IsComponentAlive)
                {
                    return;
                }

                SendActorChatMessage(serverActor, line.Type, parts[index]);

                if (!isLastLine || index < parts.Count - 1)
                {
                    await Task.Delay(ChatTypingDelay.Get(parts[index]));
                }
            }
        }

        private void SendActorChatMessage(ServerActor serverActor, PlayerChatMessageType type, string text)
        {
            // The game actor is briefly missing while an admin's edit recreates it.
            Actor? actor = serverActor.GetActor();
            if (actor == null)
            {
                return;
            }
            string name = serverActor.GetName();

            switch (type)
            {
                case PlayerChatMessageType.ME:
                    _chatService.SendActionMessage(name, actor.Position, actor.VirtualWorld, text);
                    break;
                case PlayerChatMessageType.DO:
                    _chatService.SendDoMessage(name, actor.Position, actor.VirtualWorld, text);
                    break;
                default:
                    _chatService.SendTalkMessage(name, actor.Position, text, virtualWorld: actor.VirtualWorld);
                    break;
            }
        }
    }
}
