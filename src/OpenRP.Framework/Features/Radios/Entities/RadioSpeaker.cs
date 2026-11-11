using OpenRP.Framework.Features.Radios.Components;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Radios.Entities
{
    /// <summary>
    /// A radio that's on, with where it is and who holds it: the carrier of a handheld, the people inside a vehicle,
    /// or nobody for a dropped radio or an empty vehicle.
    /// </summary>
    public class RadioSpeaker
    {
        public required Radio Radio { get; init; }
        public required Vector3 Position { get; init; }
        // Null when the radio is heard in every virtual world, like a dropped item.
        public int? VirtualWorld { get; init; }
        public required List<Player> Holders { get; init; }
        // False when the holder listens through an earpiece.
        public required bool PlaysAloud { get; init; }
    }
}
