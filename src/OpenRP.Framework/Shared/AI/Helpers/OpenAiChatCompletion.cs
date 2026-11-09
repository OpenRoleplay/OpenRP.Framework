using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace OpenRP.Framework.Shared.AI.Helpers
{
    /// <summary>
    /// Creates Semantic Kernel's OpenAI chat service and request settings, the same way for every feature that uses AI.
    /// </summary>
    public static class OpenAiChatCompletion
    {
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
        /// </summary>
        public static OpenAIPromptExecutionSettings CreateExecutionSettings(string? reasoningEffort)
        {
            return new OpenAIPromptExecutionSettings
            {
                ReasoningEffort = String.IsNullOrWhiteSpace(reasoningEffort) ? null : reasoningEffort
            };
        }
    }
}
