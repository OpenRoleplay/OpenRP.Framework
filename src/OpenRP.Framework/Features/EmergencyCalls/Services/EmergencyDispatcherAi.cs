using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenRP.Framework.Features.EmergencyCalls.Components;
using OpenRP.Framework.Features.EmergencyCalls.Entities;
using OpenRP.Framework.Features.EmergencyCalls.Helpers;
using OpenRP.Framework.Shared.AI.Helpers;
using System;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.EmergencyCalls.Services
{
    public class EmergencyDispatcherAi : IEmergencyDispatcherAi
    {
        private const string UsageFeatureName = "EmergencyCalls";

        private readonly EmergencyCallOptions _options;
        private readonly IChatCompletionService? _chatCompletionService;
        private readonly ILogger<EmergencyDispatcherAi> _logger;

        public EmergencyDispatcherAi(IOptions<EmergencyCallOptions> options, ILogger<EmergencyDispatcherAi> logger)
        {
            _options = options.Value;
            _logger = logger;

            if (String.IsNullOrWhiteSpace(_options.OpenAiApiKey))
            {
                logger.LogWarning("No OpenAI API key is set for emergency calls, so 911 callers fill in a form instead of talking to a dispatcher.");
                return;
            }

            _chatCompletionService = OpenAiChatCompletion.Create(_options.OpenAiApiKey, _options.ModelId);
        }

        public bool IsConfigured()
        {
            return _chatCompletionService != null;
        }

        public async Task<EmergencyDispatcherReply> GenerateReplyAsync(EmergencyCall call)
        {
            if (_chatCompletionService == null)
            {
                return new EmergencyDispatcherReply();
            }

            // One cache per call: each turn sends the whole call so far, which only grows.
            ChatMessageContent reply = await _chatCompletionService.GetChatMessageContentAsync(CreateChatHistory(call),
                OpenAiChatCompletion.CreateExecutionSettings(_options.ReasoningEffort, $"call:{call.Entity}"));
            OpenAiChatCompletion.LogUsage(_logger, UsageFeatureName, reply);
            return EmergencyDispatcherReplyParser.Parse(reply.Content ?? string.Empty);
        }

        /// <summary>
        /// The rules, then the caller's lines as user messages and the dispatcher's own as assistant messages. The
        /// dispatcher's lines are kept in the reply format, so the model also sees what it already dispatched.
        /// </summary>
        private static ChatHistory CreateChatHistory(EmergencyCall call)
        {
            ChatHistory chatHistory = new ChatHistory(EmergencyDispatcherPromptBuilder.Build(call.GetCallerNumber(), call.GetArea()));
            foreach (EmergencyCallLine line in call.GetLines())
            {
                if (line.IsDispatcher)
                {
                    chatHistory.AddAssistantMessage(line.Text);
                }
                else
                {
                    chatHistory.AddUserMessage(line.Text);
                }
            }
            return chatHistory;
        }
    }
}
