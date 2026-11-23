using System;

namespace OpenRP.Framework.Features.Maps.Entities
{
    /// <summary>
    /// What a bulk replace did. A null error means the objects are saved and in the game. When the error is set, read
    /// <see cref="DatabaseUnchanged"/>: when it's false, the database holds the new objects even though the game doesn't show them all.
    /// <see cref="Refused"/> is true when the replace never started, so nothing was changed.
    /// </summary>
    public sealed record MapBulkResult(string? Error, int ObjectCount, int MaterialCount, TimeSpan DatabaseTime, TimeSpan SpawnTime,
        bool DatabaseUnchanged, bool Refused)
    {
        /// <summary>
        /// A replace that failed before its rows were committed. The database still holds the old objects.
        /// </summary>
        public static MapBulkResult Failed(string error)
        {
            return new MapBulkResult(error, 0, 0, TimeSpan.Zero, TimeSpan.Zero, DatabaseUnchanged: true, Refused: false);
        }

        /// <summary>
        /// A replace that was refused before it started, for example because the map is busy. Nothing was changed.
        /// </summary>
        public static MapBulkResult Refuse(string error)
        {
            return new MapBulkResult(error, 0, 0, TimeSpan.Zero, TimeSpan.Zero, DatabaseUnchanged: true, Refused: true);
        }
    }
}
