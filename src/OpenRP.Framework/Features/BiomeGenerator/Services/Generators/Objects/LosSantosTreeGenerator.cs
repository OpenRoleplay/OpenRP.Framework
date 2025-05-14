using OpenRP.Framework.Features.BiomeGenerator.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Services.Generators.Objects
{
    public class LosSantosTreeGenerator : IBiomeObjectGenerator
    {
        public string ObjectType => "LosSantosTree";

        public BiomeObject Generate(Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation, Vector3 maxAngleRotation, Color outputColor)
        {
            int[] obj_arr_trees = { 620, 621, 712, 740, 645, 739, 710 };

            int modelId = obj_arr_trees[Random.Shared.Next(obj_arr_trees.Length)];

            BiomeObject treeObject = new BiomeObject(
                obj_arr_trees[Random.Shared.Next(obj_arr_trees.Length)],
                virtualPosition,
                gamePosition,
                maxAngleRotation,
                outputColor
            );

            return treeObject;
        }
    }
}
