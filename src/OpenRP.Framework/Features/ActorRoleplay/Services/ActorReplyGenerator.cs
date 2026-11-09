using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenRP.Framework.Features.ActorRoleplay.Entities;
using OpenRP.Framework.Features.ActorRoleplay.Helpers;
using OpenRP.Framework.Shared.AI.Helpers;

namespace OpenRP.Framework.Features.ActorRoleplay.Services
{
    /// <summary>
    /// Generates actor replies with Semantic Kernel. One chat service (and so one HTTP connection pool) is shared by
    /// every actor.
    /// </summary>
    public class ActorReplyGenerator : IActorReplyGenerator
    {
        private const string NothingNewMessage = "(Nothing new has happened. It's your turn.)";

        private readonly ActorRoleplayOptions _options;
        private readonly IChatCompletionService? _chatCompletionService;

        public ActorReplyGenerator(IOptions<ActorRoleplayOptions> options, ILogger<ActorReplyGenerator> logger)
        {
            _options = options.Value;

            if (String.IsNullOrWhiteSpace(_options.OpenAiApiKey))
            {
                logger.LogWarning("No OpenAI API key is set for actor roleplay, so AI actors won't reply.");
                return;
            }

            _chatCompletionService = OpenAiChatCompletion.Create(_options.OpenAiApiKey, _options.ModelId);
        }

        public bool IsConfigured()
        {
            return _chatCompletionService != null;
        }

        public async Task<List<ActorRoleplayLine>> GenerateReplyAsync(string actorName, string actorPrompt, IReadOnlyList<ActorMemoryLine> lines, string? situation = null)
        {
            if (_chatCompletionService == null)
            {
                return new List<ActorRoleplayLine>();
            }

            ChatHistory chatHistory = CreateChatHistory(actorName, actorPrompt, lines, situation);
            ChatMessageContent reply = await _chatCompletionService.GetChatMessageContentAsync(chatHistory,
                OpenAiChatCompletion.CreateExecutionSettings(_options.ReasoningEffort));
            return ActorReplyParser.Parse(reply.Content ?? "", actorName);
        }

        /// <summary>
        /// The system prompt, then what the actor saw and heard as user messages and its own lines as assistant
        /// messages, with consecutive lines of the same side grouped into one message.
        /// </summary>
        private static ChatHistory CreateChatHistory(string actorName, string actorPrompt, IReadOnlyList<ActorMemoryLine> lines, string? situation)
        {
            ChatHistory chatHistory = new ChatHistory(ActorRoleplayPromptBuilder.Build(actorName, actorPrompt, situation));

            int index = 0;
            while (index < lines.Count)
            {
                bool isOwnLine = lines[index].IsOwnLine;
                List<string> group = lines.Skip(index).TakeWhile(line => line.IsOwnLine == isOwnLine).Select(line => line.Text).ToList();
                AddMessage(chatHistory, isOwnLine, String.Join("\n", group));
                index += group.Count;
            }

            // The model answers the last user message; when nothing new was said (the start of a scene), it still needs one.
            if (lines.Count == 0 || lines[^1].IsOwnLine)
            {
                chatHistory.AddUserMessage(NothingNewMessage);
            }

            return chatHistory;
        }

        private static void AddMessage(ChatHistory chatHistory, bool isOwnLine, string content)
        {
            if (isOwnLine)
            {
                chatHistory.AddAssistantMessage(content);
            }
            else
            {
                chatHistory.AddUserMessage(content);
            }
        }
    }
}
