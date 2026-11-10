using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenRP.Framework.Features.ActorRoleplay.Components;
using OpenRP.Framework.Features.ActorRoleplay.Entities;
using OpenRP.Framework.Features.ActorRoleplay.Helpers;
using OpenRP.Framework.Features.Actors.Components;
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
        private static readonly TimeSpan QuietTimeBeforeReply = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan QuietTimeBeforeForgetting = TimeSpan.FromMinutes(15);

        private readonly IEntityManager _entityManager;
        private readonly IChatService _chatService;
        private readonly IActorReplyGenerator _replyGenerator;
        private readonly ActorRoleplayOptions _options;
        private readonly ILogger<ActorRoleplayService> _logger;

        // Only touched on the main thread, so a plain counter is enough.
        private int _repliesInProgress;

        public ActorRoleplayService(IEntityManager entityManager, IChatService chatService, IActorReplyGenerator replyGenerator,
            IOptions<ActorRoleplayOptions> options, ILogger<ActorRoleplayService> logger)
        {
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
                    RememberHeardLine(serverActor, heardLine);
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

        private static void RememberHeardLine(ServerActor serverActor, string heardLine)
        {
            ActorRoleplayMemory memory = serverActor.GetComponent<ActorRoleplayMemory>() ?? serverActor.AddComponent<ActorRoleplayMemory>();
            memory.AddHeardLine(heardLine);

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

            if (memory.IsReplying() || !memory.IsQuietFor(QuietTimeBeforeReply))
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
                List<ActorRoleplayLine> reply = await _replyGenerator.GenerateReplyAsync(actorName, serverActor.GetActorPrompt(), memory.GetLines(),
                    cacheKey: $"actor:{serverActor.GetId()}");

                // The actor or its memory can be gone by the time the model answers.
                if (serverActor.IsComponentAlive && memory.IsComponentAlive)
                {
                    await SendReplyLinesAsync(serverActor, memory, reply);
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
