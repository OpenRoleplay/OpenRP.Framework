using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Helpers
{
    public class BiomeModelOverride
    {
        public static float GetModelZOffset(int modelId)
        {
            switch (modelId)
            {
                case 870: // Flower
                    return 0.3f;
                case 871: // Flower
                    return 0.3f;
                case 817: // Flower
                    return 0.6f;
                case 804: // Coniferous Woodland Bush
                    return 1.0f;
                case 811: // Coniferous Woodland Bush
                    return 1.0f;
                case 835: // Fallen Tree
                    return 2.25f;
                case 844: // Fallen Tree
                    return 1.6f;
            }
            return 0.0f;
        }

        public static int GetModelStreamDistance(int modelId, int currentStreamDistance)
        {
            switch (modelId)
            {
                case -1002: // Sunflower
                case -1003: // Grass
                case -1004: // Grass
                    return 75;
            }
            return currentStreamDistance;
        }
    }
}
