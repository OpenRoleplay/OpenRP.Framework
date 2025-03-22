using OpenRP.Framework.Features.BiomeGenerator.Entities;
using OpenRP.Framework.Features.BiomeGenerator.Helpers;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Services.Generators.Objects
{
    public class ConiferousDeadTreeGenerator : IBiomeObjectGenerator
    {
        public string ObjectType => "ConiferousDeadTree";

        public BiomeObject Generate(Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation, Vector3 maxAngleRotation, Color outputColor)
        {
            int[] obj_arr_deadtree = { 848, 833, 847, 837 };

            int modelId = obj_arr_deadtree[Random.Shared.Next(obj_arr_deadtree.Length)];

            float adjustedZ = gamePosition.Z + BiomeModelOverride.GetModelZOffset(modelId);

            BiomeObject deadTreeObject = new BiomeObject(
                obj_arr_deadtree[Random.Shared.Next(obj_arr_deadtree.Length)],
                virtualPosition,
                new Vector3(gamePosition.XY, adjustedZ),
                gameRotation,
                outputColor
            );

            return deadTreeObject;
        }
    }
}
