using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Collections.Generic;
using ChatTokenUsage = OpenAI.Chat.ChatTokenUsage;

namespace OpenRP.Framework.Shared.AI.Helpers
{
    /// <summary>
    /// Creates Semantic Kernel's OpenAI chat service and request settings, the same way for every feature that uses AI.
    /// </summary>
    public static class OpenAiChatCompletion
    {
        private const string UsageMetadataKey = "Usage";

        /// <summary>
        /// A chat service for the model. Create it once and share it: it keeps one HTTP connection pool.
        /// </summary>
        public static IChatCompletionService Create(string apiKey, string modelId)
        {
            return Kernel.CreateBuilder()
                .AddOpenAIChatCompletion(modelId, apiKey)
                .Build()
                .GetRequiredService<IChatCompletionService>();
        }

        /// <summary>
        /// Request settings. The reasoning effort is left out when empty, because models that don't reason reject it.
        /// The cache key groups the requests of one conversation, so OpenAI sends them to the same prompt cache.
        /// </summary>
        public static OpenAIPromptExecutionSettings CreateExecutionSettings(string? reasoningEffort, string? cacheKey = null)
        {
            Dictionary<string, object?> extraFields = new Dictionary<string, object?>();

            // Not through settings.ReasoningEffort: Semantic Kernel only knows low, medium and high, and refuses others
            // such as "none" before the request is sent. As a raw field, any value OpenAI accepts gets through.
            if (!String.IsNullOrWhiteSpace(reasoningEffort))
            {
                extraFields["reasoning_effort"] = reasoningEffort;
            }

            // The OpenAI library has no property for prompt_cache_key either.
            if (!String.IsNullOrWhiteSpace(cacheKey))
            {
                extraFields["prompt_cache_key"] = cacheKey;
            }

            OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings();
            if (extraFields.Count > 0)
            {
                // ExtraBody is marked experimental in Semantic Kernel; it's the documented way to send such fields.
#pragma warning disable SKEXP0010
                settings.ExtraBody = extraFields;
#pragma warning restore SKEXP0010
            }
            return settings;
        }

        /// <summary>
        /// Logs the tokens of one reply: input (and how much of it was cached), output and reasoning. This is how
        /// to see whether prompt caching works.
        /// </summary>
        public static void LogUsage(ILogger logger, string feature, ChatMessageContent reply)
        {
            if (reply.Metadata == null || !reply.Metadata.TryGetValue(UsageMetadataKey, out object? value) || value is not ChatTokenUsage usage)
            {
                return;
            }

            logger.LogInformation("AI usage ({Feature}): {InputTokens} in ({CachedTokens} cached), {OutputTokens} out ({ReasoningTokens} reasoning).",
                feature, usage.InputTokenCount, usage.InputTokenDetails?.CachedTokenCount ?? 0,
                usage.OutputTokenCount, usage.OutputTokenDetails?.ReasoningTokenCount ?? 0);
        }
    }
}
