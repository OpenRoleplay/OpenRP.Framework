using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenRP.Framework.Features.ActorRoleplay.Entities;
using OpenRP.Framework.Features.ActorRoleplay.Helpers;

namespace OpenRP.Framework.Features.ActorRoleplay.Services
{
    /// <summary>
    /// Generates actor replies with Semantic Kernel. One chat service (and so one HTTP connection pool) is shared by
    /// every actor.
    /// </summary>
    public class ActorReplyGenerator : IActorReplyGenerator
    {
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

            _chatCompletionService = CreateChatCompletionService();
        }

        public bool IsConfigured()
        {
            return _chatCompletionService != null;
        }

        public async Task<string?> GenerateReplyAsync(string actorName, string actorPrompt, IReadOnlyList<ActorMemoryLine> lines)
        {
            if (_chatCompletionService == null)
            {
                return null;
            }

            ChatHistory chatHistory = CreateChatHistory(actorName, actorPrompt, lines);
            ChatMessageContent reply = await _chatCompletionService.GetChatMessageContentAsync(chatHistory, CreateExecutionSettings());
            return reply.Content;
        }

        private IChatCompletionService CreateChatCompletionService()
        {
            return Kernel.CreateBuilder()
                .AddOpenAIChatCompletion(_options.ModelId, _options.OpenAiApiKey!)
                .Build()
                .GetRequiredService<IChatCompletionService>();
        }

        private OpenAIPromptExecutionSettings CreateExecutionSettings()
        {
            return new OpenAIPromptExecutionSettings
            {
                ReasoningEffort = String.IsNullOrWhiteSpace(_options.ReasoningEffort) ? null : _options.ReasoningEffort
            };
        }

        /// <summary>
        /// The system prompt, then what the actor saw and heard as user messages and its own lines as assistant
        /// messages, with consecutive lines of the same side grouped into one message.
        /// </summary>
        private static ChatHistory CreateChatHistory(string actorName, string actorPrompt, IReadOnlyList<ActorMemoryLine> lines)
        {
            ChatHistory chatHistory = new ChatHistory(ActorRoleplayPromptBuilder.Build(actorName, actorPrompt));

            int index = 0;
            while (index < lines.Count)
            {
                bool isOwnLine = lines[index].IsOwnLine;
                List<string> group = lines.Skip(index).TakeWhile(line => line.IsOwnLine == isOwnLine).Select(line => line.Text).ToList();
                AddMessage(chatHistory, isOwnLine, String.Join("\n", group));
                index += group.Count;
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
