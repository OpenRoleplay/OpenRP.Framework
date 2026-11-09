using OpenRP.Framework.Shared.Chat.Enums;

namespace OpenRP.Framework.Features.ActorConversations.Entities
{
    /// <summary>
    /// One /me, /do or /say line of a conversation, by the actor with this id.
    /// </summary>
    public record ActorConversationLine(ulong ActorId, PlayerChatMessageType Type, string Text);
}
