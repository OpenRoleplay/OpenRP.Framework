using OpenRP.Framework.Features.BiomeGenerator.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Services.Generators.Objects
{
    internal class DesertAltRockGenerator : IBiomeObjectGenerator
    {
        public string ObjectType => "DesertAltRock";

        public BiomeObject Generate(Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation, Vector3 maxAngleRotation, Color outputColor)
        {
            int[] obj_arr_rocks = { 905 };

            int modelId = obj_arr_rocks[Random.Shared.Next(obj_arr_rocks.Length)];

            BiomeObject rocksObject = new BiomeObject(
                obj_arr_rocks[Random.Shared.Next(obj_arr_rocks.Length)],
                virtualPosition,
                gamePosition,
                gameRotation,
                outputColor
            );

            return rocksObject;
        }
    }
}
