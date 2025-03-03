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
    public class DeadTreeGenerator : IBiomeObjectGenerator
    {
        public string ObjectType => "DeadTree";

        public BiomeObject Generate(Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation, Color outputColor)
        {
            int[] obj_arr_deadtree = { 848, 833, 831, 847, 837, 832, 841, 842, 836, 840, 834, 839, 843, 838, 844, 835, 846, 845 };

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
