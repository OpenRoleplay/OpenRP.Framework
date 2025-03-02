using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Entities
{
    public class GeneratedObject
    {
        public int ModelId { get; set; }
        public Vector2 VirtualPosition { get; set; }
        public Vector3 GamePosition { get; set; }
        public Vector3 GameRotation { get; set; }
        public List<GeneratedMaterial>? Materials { get; set; } = new List<GeneratedMaterial>();
        public Color Color { get; set; }
        public int? StreamDistance { get; set; }

        public GeneratedObject(int modelId, Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Color color, List<GeneratedMaterial>? materials = null, int? streamDistance = null)
        {
            ModelId = modelId;
            VirtualPosition = virtualPosition;
            GamePosition = gamePosition;
            GameRotation = gameRotation;
            if (materials is List<GeneratedMaterial>)
            {
                Materials = materials;
            }
            else
            {
                Materials = new List<GeneratedMaterial>();
            }
            Color = color;
            StreamDistance = streamDistance;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder($"ModelId: {ModelId}, Position: {GamePosition}, Rotation: {GameRotation}, Color: RGB({Color.Red}, {Color.Green}, {Color.Blue})");

            int materialIndex = 1;
            foreach (GeneratedMaterial material in Materials)
            {
                stringBuilder.Append($", [Material {materialIndex} = MaterialIndex: {material.MaterialIndex}, ModelId: {material.ModelId}, TxdName: {material.TxdName}, TextureName: {material.TextureName}, MaterialColor: RGB({material.MaterialColor.R}, {material.MaterialColor.G}, {material.MaterialColor.B})]");
                materialIndex++;
            }

            return stringBuilder.ToString();
        }
    }
}
