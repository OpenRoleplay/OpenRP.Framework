using OpenRP.Framework.Features.BiomeGenerator.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Services.Generators.Objects
{
    public class BirchTreeGenerator : IBiomeObjectGenerator
    {
        public string ObjectType => "BirchTree";

        public BiomeObject Generate(Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation, Vector3 maxAngleRotation, Color outputColor)
        {
            int[] obj_arr_trees = { 713, 763, 770, 775 };

            int modelId = obj_arr_trees[Random.Shared.Next(obj_arr_trees.Length)];

            BiomeObject treeObject = new BiomeObject(
                obj_arr_trees[Random.Shared.Next(obj_arr_trees.Length)],
                virtualPosition,
                gamePosition,
                gameRotation,
                outputColor
            );

            BiomeObjectMaterial treeMaterial;
            switch (treeObject.ModelId)
            {
                case 763:
                    treeMaterial = new BiomeObjectMaterial(0, 713, "gta_tree_boak", "sm_bark_light");
                    treeObject.Materials.Add(treeMaterial);
                    break;
                case 770:
                    treeMaterial = new BiomeObjectMaterial(0, 713, "gta_tree_boak", "sm_bark_light");
                    treeObject.Materials.Add(treeMaterial);
                    break;
                case 775:
                    treeMaterial = new BiomeObjectMaterial(0, 713, "gta_tree_boak", "sm_bark_light");
                    treeObject.Materials.Add(treeMaterial);
                    break;
            }

            return treeObject;
        }
    }
}
