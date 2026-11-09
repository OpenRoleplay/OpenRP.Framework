using OpenRP.Framework.Shared.Chat.Enums;

namespace OpenRP.Framework.Features.ActorRoleplay.Entities
{
    /// <summary>
    /// One line of an actor's reply: <see cref="PlayerChatMessageType.ME"/>, <see cref="PlayerChatMessageType.DO"/> or
    /// <see cref="PlayerChatMessageType.TALK"/>, with its text.
    /// </summary>
    public record ActorRoleplayLine(PlayerChatMessageType Type, string Text)
    {
        /// <summary>
        /// The line as the command the actor would have typed, such as "/me waves", which is how replies are remembered.
        /// </summary>
        public string GetCommandText()
        {
            return Type switch
            {
                PlayerChatMessageType.ME => $"/me {Text}",
                PlayerChatMessageType.DO => $"/do {Text}",
                _ => $"/say {Text}"
            };
        }
    }
}
