using OpenRP.Framework.Features.BiomeGenerator.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.ObjectGenerators
{
    public class GrassGenerator : IBiomeObjectGenerator
    {
        public BiomeObject Generate(Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation, Color outputColor)
        {
            int[] obj_arr_grass = { -1003, -1004 };

            int modelId = obj_arr_grass[Random.Shared.Next(obj_arr_grass.Length)];

            BiomeObject grassObject = new BiomeObject(
                obj_arr_grass[Random.Shared.Next(obj_arr_grass.Length)],
                virtualPosition,
                new Vector3(gamePosition.XY, gamePosition.Z),
                gameRotation,
                outputColor
            );

            return grassObject;
        }
    }
}
