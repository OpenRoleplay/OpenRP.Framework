﻿using OpenRP.Framework.Features.BiomeGenerator.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Services.Generators.Objects
{
    public class ElmTreeGenerator : IBiomeObjectGenerator
    {
        public string ObjectType => "ElmTree";

        public BiomeObject Generate(Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation, Vector3 maxAngleRotation, Color outputColor)
        {
            int[] obj_arr_trees = { 893, 768, 895, 767, 782, 887, 886, 772, 775, 778, 890, 781 };

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
