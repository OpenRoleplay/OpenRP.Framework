using OpenRP.Framework.Features.BiomeGenerator.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Services.Generators.Objects
{
    public class MultipleSunflowerGenerator : IBiomeObjectGenerator
    {
        public string ObjectType => "SunflowerMultiple";

        public BiomeObject Generate(Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation, Vector3 maxAngleRotation, Color outputColor)
        {
            BiomeObject sunflowerObject = new BiomeObject(
                -1000,
                virtualPosition,
                gamePosition,
                defaultRotation,
                outputColor
            );

            return sunflowerObject;
        }
    }
}
