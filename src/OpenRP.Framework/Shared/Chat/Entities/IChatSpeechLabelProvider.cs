using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Shared.Chat.Entities
{
    /// <summary>
    /// Tells the chat what a player is talking into, so people nearby see it, for example "Name says (phone): text".
    /// Register one with AddChatSpeechLabelProvider&lt;T&gt;().
    /// </summary>
    public interface IChatSpeechLabelProvider
    {
        /// <summary>
        /// The label for the player's speech, such as "phone", or null when this provider has nothing to add.
        /// </summary>
        string? GetSpeechLabel(Player player);
    }
}
