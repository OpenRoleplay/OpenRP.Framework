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
    public class DeadTreeStandingGenerator : IBiomeObjectGenerator
    {
        public string ObjectType => "DeadTreeStanding";

        public BiomeObject Generate(Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation, Vector3 maxAngleRotation, Color outputColor)
        {
            int[] obj_arr_deadtree_standing = { 704, 686 };

            int modelId = obj_arr_deadtree_standing[Random.Shared.Next(obj_arr_deadtree_standing.Length)];

            float adjustedZ = gamePosition.Z + BiomeModelOverride.GetModelZOffset(modelId);

            BiomeObject deadTreeStandingObject = new BiomeObject(
                obj_arr_deadtree_standing[Random.Shared.Next(obj_arr_deadtree_standing.Length)],
                virtualPosition,
                new Vector3(gamePosition.XY, adjustedZ),
                gameRotation,
                outputColor
            );

            return deadTreeStandingObject;
        }
    }
}
