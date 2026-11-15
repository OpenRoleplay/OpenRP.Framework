using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Maps.Helpers
{
    /// <summary>
    /// Runs work over a big list of map objects in slices, so the main thread gets a turn between them.
    /// </summary>
    public static class MapSlices
    {
        public const int SliceSize = 20000;

        /// <summary>
        /// Runs the work for every item, a slice at a time, and reports how many items are done after each slice.
        /// Continues on the main thread, because the wait between slices uses SampSharp's synchronization context.
        /// </summary>
        public static async Task RunAsync<T>(IReadOnlyList<T> items, Action<T> work, IProgress<int>? progress = null)
        {
            for (int sliceStart = 0; sliceStart < items.Count; sliceStart += SliceSize)
            {
                int sliceEnd = Math.Min(sliceStart + SliceSize, items.Count);
                for (int index = sliceStart; index < sliceEnd; index++)
                {
                    work(items[index]);
                }
                progress?.Report(sliceEnd);
                if (sliceEnd < items.Count)
                {
                    await Task.Delay(1);
                }
            }
        }
    }
}
