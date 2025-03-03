using OpenRP.Framework.Features.BiomeGenerator.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Services.Generators.Objects
{
    internal class RubbleGenerator : IBiomeObjectGenerator
    {
        public string ObjectType => "Rubble";

        public BiomeObject Generate(Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation, Color outputColor)
        {
            int[] obj_arr_rubble = { 807, 906, 880, 879, 816, 828, 867, 868 };

            int modelId = obj_arr_rubble[Random.Shared.Next(obj_arr_rubble.Length)];

            BiomeObject rubbleObject = new BiomeObject(
                obj_arr_rubble[Random.Shared.Next(obj_arr_rubble.Length)],
                virtualPosition,
                gamePosition,
                gameRotation,
                outputColor
            );

            return rubbleObject;
        }
    }
}
