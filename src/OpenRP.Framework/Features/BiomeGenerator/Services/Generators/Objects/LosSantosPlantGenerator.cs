using OpenRP.Framework.Features.BiomeGenerator.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Services.Generators.Objects
{
    public class LosSantosPlantGenerator : IBiomeObjectGenerator
    {
        public string ObjectType => "LosSantosPlant";

        public BiomeObject Generate(Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation, Vector3 maxAngleRotation, Color outputColor)
        {
            int[] obj_arr_plants = { -1005, -1006, -1007, -1009, 692, 859, 860, 861, 862 };

            int modelId = obj_arr_plants[Random.Shared.Next(obj_arr_plants.Length)];

            BiomeObject plantObject = new BiomeObject(
                obj_arr_plants[Random.Shared.Next(obj_arr_plants.Length)],
                virtualPosition,
                gamePosition,
                gameRotation,
                outputColor
            );

            return plantObject;
        }
    }
}
