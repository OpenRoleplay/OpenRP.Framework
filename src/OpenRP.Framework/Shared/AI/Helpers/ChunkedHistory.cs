namespace OpenRP.Framework.Shared.AI.Helpers
{
    /// <summary>
    /// Which part of a long history is sent to the model. The window grows to MaxLines, then jumps forward in one go to
    /// the newest LinesKeptAfterTrim lines, instead of sliding by one line per request.
    /// </summary>
    /// <remarks>
    /// OpenAI caches a request by its exact start. Between jumps, every request starts the same way as the one before,
    /// so everything but the newest lines is billed at the cached rate.
    /// </remarks>
    public static class ChunkedHistory
    {
        public const int MaxLines = 40;
        public const int LinesKeptAfterTrim = 20;

        /// <summary>
        /// The index of the first line to send, out of a history of lineCount lines, oldest first.
        /// </summary>
        public static int GetWindowStart(int lineCount)
        {
            if (lineCount <= MaxLines)
            {
                return 0;
            }

            int step = MaxLines - LinesKeptAfterTrim;
            return (lineCount - LinesKeptAfterTrim - 1) / step * step;
        }
    }
}
