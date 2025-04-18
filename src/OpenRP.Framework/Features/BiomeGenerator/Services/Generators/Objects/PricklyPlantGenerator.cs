using OpenRP.Framework.Features.BiomeGenerator.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Services.Generators.Objects
{
    public class PricklyPlantGenerator : IBiomeObjectGenerator
    {
        public string ObjectType => "PricklyPlant";

        public BiomeObject Generate(Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation, Vector3 maxAngleRotation, Color outputColor)
        {
            int[] obj_arr_plants = { 755, 756, 757 };

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
