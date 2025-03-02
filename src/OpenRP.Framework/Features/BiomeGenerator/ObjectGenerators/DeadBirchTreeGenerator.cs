using OpenRP.Framework.Features.BiomeGenerator.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.ObjectGenerators
{
    public class DeadBirchTreeGenerator : IBiomeObjectGenerator
    {
        public BiomeObject Generate(Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation, Color outputColor)
        {
            DeadBirchTreeGenerator deadBirchTreeGenerator = new DeadBirchTreeGenerator();
            BiomeObject deadObject = deadBirchTreeGenerator.Generate(virtualPosition, gamePosition, gameRotation, defaultRotation, outputColor);

            BiomeObjectMaterial deadMaterial;
            switch (deadObject.ModelId)
            {
                case 848:
                    deadMaterial = new BiomeObjectMaterial(0, 713, "gta_tree_boak", "sm_bark_light");
                    deadObject.Materials.Add(deadMaterial);
                    deadMaterial = new BiomeObjectMaterial(1, 713, "gta_tree_boak", "sm_bark_light");
                    deadObject.Materials.Add(deadMaterial);
                    break;
                case 833:
                    deadMaterial = new BiomeObjectMaterial(1, 713, "gta_tree_boak", "sm_bark_light");
                    deadObject.Materials.Add(deadMaterial);
                    break;
                case 847:
                    deadMaterial = new BiomeObjectMaterial(0, 713, "gta_tree_boak", "sm_bark_light");
                    deadObject.Materials.Add(deadMaterial);
                    break;
                case 837:
                    deadMaterial = new BiomeObjectMaterial(0, 713, "gta_tree_boak", "sm_bark_light");
                    deadObject.Materials.Add(deadMaterial);
                    break;
                case 831:
                    deadMaterial = new BiomeObjectMaterial(0, 713, "gta_tree_boak", "sm_bark_light", Color.White);
                    deadObject.Materials.Add(deadMaterial);
                    break;
                case 832:
                    deadMaterial = new BiomeObjectMaterial(0, 713, "gta_tree_boak", "sm_bark_light");
                    deadObject.Materials.Add(deadMaterial);
                    break;
                case 841:
                    deadMaterial = new BiomeObjectMaterial(0, 713, "gta_tree_boak", "sm_bark_light");
                    deadObject.Materials.Add(deadMaterial);
                    break;
                case 842:
                    deadMaterial = new BiomeObjectMaterial(0, 713, "gta_tree_boak", "sm_bark_light");
                    deadObject.Materials.Add(deadMaterial);
                    break;
                case 836:
                    deadMaterial = new BiomeObjectMaterial(0, 713, "gta_tree_boak", "sm_bark_light");
                    deadObject.Materials.Add(deadMaterial);
                    break;
                case 840:
                    deadMaterial = new BiomeObjectMaterial(0, 713, "gta_tree_boak", "sm_bark_light");
                    deadObject.Materials.Add(deadMaterial);
                    deadMaterial = new BiomeObjectMaterial(1, 713, "gta_tree_boak", "sm_bark_light");
                    deadObject.Materials.Add(deadMaterial);
                    break;
                case 834:
                    deadMaterial = new BiomeObjectMaterial(0, 713, "gta_tree_boak", "sm_bark_light");
                    deadObject.Materials.Add(deadMaterial);
                    break;
                case 839:
                    deadMaterial = new BiomeObjectMaterial(0, 713, "gta_tree_boak", "sm_bark_light");
                    deadObject.Materials.Add(deadMaterial);
                    break;
                case 843:
                    deadMaterial = new BiomeObjectMaterial(0, 713, "gta_tree_boak", "sm_bark_light");
                    deadObject.Materials.Add(deadMaterial);
                    break;
                case 838:
                    deadMaterial = new BiomeObjectMaterial(0, 713, "gta_tree_boak", "sm_bark_light");
                    deadObject.Materials.Add(deadMaterial);
                    break;
                case 844:
                    deadMaterial = new BiomeObjectMaterial(0, 713, "gta_tree_boak", "sm_bark_light");
                    deadObject.Materials.Add(deadMaterial);
                    break;
                case 835:
                    deadMaterial = new BiomeObjectMaterial(0, 713, "gta_tree_boak", "sm_bark_light");
                    deadObject.Materials.Add(deadMaterial);
                    break;
                case 846:
                    deadMaterial = new BiomeObjectMaterial(0, 713, "gta_tree_boak", "sm_bark_light");
                    deadObject.Materials.Add(deadMaterial);
                    break;
                case 845:
                    deadMaterial = new BiomeObjectMaterial(0, 713, "gta_tree_boak", "sm_bark_light");
                    deadObject.Materials.Add(deadMaterial);
                    break;
            }

            return deadObject;
        }
    }
}
