using System.Collections.Generic;

namespace OpenRP.Framework.Features.Maps.Entities
{
    /// <summary>
    /// The ids of the rows a bulk replace inserted. The objects are in the order they were given. The materials are in
    /// the order of their objects, and each object's textures are in the order they were given.
    /// </summary>
    public sealed record MapBulkRowIds(IReadOnlyList<ulong> ObjectIds, IReadOnlyList<ulong> MaterialIds);
}
