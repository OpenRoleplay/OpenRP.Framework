namespace OpenRP.Framework.Shared.Chat.Helpers
{
    /// <summary>
    /// How long to pause after a line written by an actor, as if a person had typed it, so lines don't arrive all at once.
    /// </summary>
    public static class ChatTypingDelay
    {
        private const int MinimumDelayInMilliseconds = 1000;
        private const int DelayPerCharacterInMilliseconds = 40;
        private const int MaximumDelayInMilliseconds = 5000;

        public static TimeSpan Get(string text)
        {
            int delay = MinimumDelayInMilliseconds + text.Length * DelayPerCharacterInMilliseconds;
            return TimeSpan.FromMilliseconds(Math.Min(delay, MaximumDelayInMilliseconds));
        }
    }
}
