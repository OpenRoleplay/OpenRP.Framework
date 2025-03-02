using SampSharp.Entities.SAMP;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Entities
{
    public interface IBiome
    {
        ConcurrentBag<BiomeObject> Generate(Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation);
        Color GetBiomeOutputColor();
    }
}
