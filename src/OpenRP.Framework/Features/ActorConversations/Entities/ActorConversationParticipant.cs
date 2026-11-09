namespace OpenRP.Framework.Features.ActorConversations.Entities
{
    /// <summary>
    /// An actor taking part in a conversation, copied when the conversation is created so it doesn't depend on the
    /// actor's component later.
    /// </summary>
    public record ActorConversationParticipant(ulong ActorId, string Name, string Prompt);
}
