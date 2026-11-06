using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Factions.Services
{
    public interface IFactionChatService
    {
        /// <summary>
        /// Sends an out-of-character message to the online members of the player's active faction, or tells the player
        /// why it can't be sent.
        /// </summary>
        void SendFactionOocMessage(Player player, string text);
    }
}
