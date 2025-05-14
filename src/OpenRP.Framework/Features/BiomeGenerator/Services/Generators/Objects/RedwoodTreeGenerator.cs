using OpenRP.Framework.Features.BiomeGenerator.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Services.Generators.Objects
{
    public class RedwoodTreeGenerator : IBiomeObjectGenerator
    {
        public string ObjectType => "RedwoodTree";

        public BiomeObject Generate(Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation, Vector3 maxAngleRotation, Color outputColor)
        {
            int[] obj_arr_trees = { 726, 731 };

            int modelId = obj_arr_trees[Random.Shared.Next(obj_arr_trees.Length)];

            BiomeObject treeObject = new BiomeObject(
                obj_arr_trees[Random.Shared.Next(obj_arr_trees.Length)],
                virtualPosition,
                gamePosition,
                gameRotation,
                outputColor
            );

            return treeObject;
        }
    }
}
