using OpenRP.Framework.Features.BiomeGenerator.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Services.Generators.Objects
{
    internal class SmallRockGenerator : IBiomeObjectGenerator
    {
        public string ObjectType => "SmallRock";

        public BiomeObject Generate(Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation, Vector3 maxAngleRotation, Color outputColor)
        {
            int[] obj_arr_gravel = { 905, 807 };

            int modelId = obj_arr_gravel[Random.Shared.Next(obj_arr_gravel.Length)];

            BiomeObject gravelObject = new BiomeObject(
                obj_arr_gravel[Random.Shared.Next(obj_arr_gravel.Length)],
                virtualPosition,
                gamePosition,
                gameRotation,
                outputColor
            );

            BiomeObjectMaterial gravelMaterial = new BiomeObjectMaterial(0, 896, "underwater", "greyrockbig");
            gravelObject.Materials.Add(gravelMaterial);

            return gravelObject;
        }
    }
}
