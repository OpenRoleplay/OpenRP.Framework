using OpenRP.Framework.Features.BiomeGenerator.Entities;
using OpenRP.Framework.Features.BiomeGenerator.Helpers;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Services.ObjectGenerators
{
    public class FlowerGenerator : IBiomeObjectGenerator
    {
        public string ObjectType => "Flower";

        public BiomeObject Generate(Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation, Color outputColor)
        {
            int[] obj_arr_flower = { 870, 871, 817 };

            int modelId = obj_arr_flower[Random.Shared.Next(obj_arr_flower.Length)];

            float adjustedZ = gamePosition.Z + BiomeModelOverride.GetModelZOffset(modelId);

            BiomeObject flowerObject = new BiomeObject(
                modelId,
                virtualPosition,
                new Vector3(gamePosition.XY, adjustedZ),
                gameRotation,
                outputColor
            );

            if (modelId == 870)
            {
                if (Random.Shared.Next(2) == 0)
                {
                    BiomeObjectMaterial generatedMaterial = new BiomeObjectMaterial(0, 13861, "lahills_wiresnshit3", "bevflower2");
                    flowerObject.Materials.Add(generatedMaterial);
                }
            }

            return flowerObject;
        }
    }
}
