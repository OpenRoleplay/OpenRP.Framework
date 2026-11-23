using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Maps.Enums;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Maps.Components
{
    /// <summary>
    /// What one material slot of a map object shows, either a texture or text. A child entity of the object. Only the map
    /// store changes its state.
    /// </summary>
    public class MapObjectMaterial : Component
    {
        /// <summary>
        /// Objects have 16 material slots, 0 to 15.
        /// </summary>
        public const int MaxMaterialIndex = 15;

        private readonly MapObjectMaterialModel _materialModel;

        public MapObjectMaterial(MapObjectMaterialModel materialModel)
        {
            _materialModel = materialModel;
        }

        public MapObjectMaterialModel GetRawMapObjectMaterialModel()
        {
            return _materialModel;
        }

        public ulong GetId()
        {
            return _materialModel.Id;
        }

        public int GetMaterialIndex()
        {
            return _materialModel.MaterialIndex;
        }

        public MapObjectMaterialType GetMaterialType()
        {
            return _materialModel.Type;
        }

        public int? GetTextureModelId()
        {
            return _materialModel.ModelId;
        }

        public string? GetTxdName()
        {
            return _materialModel.TxdName;
        }

        public string? GetTextureName()
        {
            return _materialModel.TextureName;
        }

        public uint GetColor()
        {
            return _materialModel.Color;
        }

        public string? GetText()
        {
            return _materialModel.Text;
        }

        public int GetMaterialSize()
        {
            return _materialModel.MaterialSize;
        }

        public string? GetFontFace()
        {
            return _materialModel.FontFace;
        }

        public int GetFontSize()
        {
            return _materialModel.FontSize;
        }

        public bool GetBold()
        {
            return _materialModel.Bold;
        }

        public uint GetFontColor()
        {
            return _materialModel.FontColor;
        }

        public uint GetBackColor()
        {
            return _materialModel.BackColor;
        }

        public int GetAlignment()
        {
            return _materialModel.Alignment;
        }

        /// <summary>
        /// Replaces what the slot shows with the content of another material. The slot index and the object stay.
        /// </summary>
        internal void CopyContentFrom(MapObjectMaterialModel content)
        {
            _materialModel.Type = content.Type;
            _materialModel.ModelId = content.ModelId;
            _materialModel.TxdName = content.TxdName;
            _materialModel.TextureName = content.TextureName;
            _materialModel.Color = content.Color;
            _materialModel.Text = content.Text;
            _materialModel.MaterialSize = content.MaterialSize;
            _materialModel.FontFace = content.FontFace;
            _materialModel.FontSize = content.FontSize;
            _materialModel.Bold = content.Bold;
            _materialModel.FontColor = content.FontColor;
            _materialModel.BackColor = content.BackColor;
            _materialModel.Alignment = content.Alignment;
        }
    }
}
