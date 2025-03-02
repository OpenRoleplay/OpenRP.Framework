using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Entities
{
    public class GeneratedMaterial
    {
        public int MaterialIndex { get; set; }
        public int ModelId { get; set; }
        public string TxdName { get; set; }
        public string TextureName { get; set; }
        public Color MaterialColor { get; set; }

        public GeneratedMaterial(int materialIndex, int modelId, string txdName, string textureName, Color materialColor = new Color())
        {
            MaterialIndex = materialIndex;
            ModelId = modelId;
            TxdName = txdName;
            TextureName = textureName;
            MaterialColor = materialColor;
        }
    }
}
