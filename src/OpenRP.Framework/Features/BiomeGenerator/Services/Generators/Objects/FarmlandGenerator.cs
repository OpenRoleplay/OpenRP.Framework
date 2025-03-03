using OpenRP.Framework.Features.BiomeGenerator.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Services.Generators.Objects
{
    public class FarmlandGenerator : IBiomeObjectGenerator
    {
        public string ObjectType => "Farmland";

        public BiomeObject Generate(Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation, Color outputColor)
        {
            BiomeObject generatedObject = new BiomeObject(855, virtualPosition, gamePosition, gameRotation, outputColor);

            return generatedObject;
        }
    }
}
