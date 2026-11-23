using OpenRP.Framework.Database.Entities;
using OpenRP.Framework.Features.Maps.Enums;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// What one material slot (0 to 15) of a map object shows: a texture from a model's TXD, or text.
    /// </summary>
    public class MapObjectMaterialModel : BaseModel
    {
        public ulong MapObjectId { get; set; }
        public int MaterialIndex { get; set; }
        public MapObjectMaterialType Type { get; set; }

        // Texture columns, used when Type is Texture.
        public int? ModelId { get; set; }
        public string? TxdName { get; set; }
        public string? TextureName { get; set; }

        // ARGB; 0 keeps the original colour.
        public uint Color { get; set; }

        // Text columns, used when Type is Text.
        public string? Text { get; set; }
        public int MaterialSize { get; set; }
        public string? FontFace { get; set; }
        public int FontSize { get; set; }
        public bool Bold { get; set; }
        public uint FontColor { get; set; }
        public uint BackColor { get; set; }
        public int Alignment { get; set; }

        // Navigational Properties
        public MapObjectModel? MapObject { get; set; }
    }
}
