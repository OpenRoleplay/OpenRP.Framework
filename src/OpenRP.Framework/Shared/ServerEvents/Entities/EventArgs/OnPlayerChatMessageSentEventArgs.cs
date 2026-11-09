using OpenRP.Framework.Shared.Chat.Enums;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs
{
    /// <summary>
    /// A player's character said or did something in chat that others nearby could see or hear, for example /me, /do or /say.
    /// </summary>
    public class OnPlayerChatMessageSentEventArgs : ServerEventArgs
    {
        public Player? Player { get; set; }
        public string CharacterName { get; set; } = string.Empty;
        public string? Accent { get; set; }
        public PlayerChatMessageType Type { get; set; }
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Where the message was sent from; anyone within <see cref="Range"/> in the same virtual world saw it.
        /// </summary>
        public Vector3 Position { get; set; }
        public int VirtualWorld { get; set; }
        public float Range { get; set; }
    }
}
