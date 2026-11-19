using SampSharp.Entities;
using System;
using System.Numerics;

namespace OpenRP.Framework.Features.Fishing.Entities
{
    /// <summary>
    /// Where and when a line was cast. The boat is null when the player fishes from land.
    /// </summary>
    public record FishingCast(Vector3 Position, EntityId? BoatEntityId, DateTime BiteAt);
}
