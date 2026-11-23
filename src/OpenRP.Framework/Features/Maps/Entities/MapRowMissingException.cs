using System;

namespace OpenRP.Framework.Features.Maps.Entities
{
    /// <summary>
    /// A change found no row to change in the database, because another server deleted or replaced it.
    /// </summary>
    public class MapRowMissingException : Exception
    {
        public MapRowMissingException(string thing)
            : base($"That {thing} no longer exists in the database (changed on another server). Restart to reload.")
        {
        }
    }
}
