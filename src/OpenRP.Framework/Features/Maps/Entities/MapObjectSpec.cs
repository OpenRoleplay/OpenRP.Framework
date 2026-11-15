using System.Collections.Generic;

namespace OpenRP.Framework.Features.Maps.Entities
{
    /// <summary>
    /// One object of a bulk replace. Negative models are custom models, and model 0 is refused. The virtual world and
    /// interior are -1 for everywhere. A null stream distance means the default, see <see cref="MapLimits"/>.
    /// </summary>
    public sealed record MapObjectSpec(int ModelId, Vector3 Position, Vector3 Rotation,
        int VirtualWorld = MapLimits.EverywhereValue, int Interior = MapLimits.EverywhereValue,
        float? StreamDistance = null, string? Description = null, IReadOnlyList<MapObjectTextureSpec>? Textures = null);
}
