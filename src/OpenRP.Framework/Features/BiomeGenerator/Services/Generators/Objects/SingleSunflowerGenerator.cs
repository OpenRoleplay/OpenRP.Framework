using OpenRP.Framework.Features.BiomeGenerator.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Services.Generators.Objects
{
    public class SingleSunflowerGenerator : IBiomeObjectGenerator
    {
        public string ObjectType => "Sunflower";

        public BiomeObject Generate(Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation, Color outputColor)
        {
            BiomeObject sunflowerObject = new BiomeObject(
                -1002,
                virtualPosition,
                gamePosition,
                defaultRotation,
                outputColor
            );

            return sunflowerObject;
        }
    }
}
