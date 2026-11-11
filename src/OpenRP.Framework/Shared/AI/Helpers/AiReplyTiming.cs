using System;

namespace OpenRP.Framework.Shared.AI.Helpers
{
    /// <summary>
    /// When an AI character (an actor, the 911 dispatcher) answers a player.
    /// </summary>
    public static class AiReplyTiming
    {
        /// <summary>
        /// How long the player has to be quiet before the AI answers. SA-MP doesn't tell the server when someone is
        /// typing, so this pause is what gives them time to type a follow-up line, which then gets the same answer.
        /// </summary>
        public static readonly TimeSpan QuietTimeBeforeReply = TimeSpan.FromSeconds(5);
    }
}
