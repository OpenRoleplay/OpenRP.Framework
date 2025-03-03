using OpenRP.Framework.Features.BiomeGenerator.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Services
{
    public interface IBiomeObjectGenerator
    {
        string ObjectType { get; }
        BiomeObject Generate(Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation, Color outputColor);
    }
}
