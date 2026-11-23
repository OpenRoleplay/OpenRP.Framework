using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Maps.Entities
{
    /// <summary>
    /// A texture on one material slot (0 to 15) of an object of a bulk replace. The colour is ARGB, and 0 keeps the
    /// original colour.
    /// </summary>
    public sealed record MapObjectTextureSpec(int MaterialIndex, int ModelId, string TxdName, string TextureName, uint ArgbColor)
    {
        /// <summary>
        /// A texture whose colour is packed the way the streamer packs material colours.
        /// </summary>
        public static MapObjectTextureSpec FromColor(int index, int modelId, string txd, string texture, Color color)
        {
            return new MapObjectTextureSpec(index, modelId, txd, texture, PackArgb(color));
        }

        /// <summary>
        /// The colour as 0xAARRGGBB, with the alpha in the high byte.
        /// </summary>
        private static uint PackArgb(Color color)
        {
            return ((uint)color.A << 24) | ((uint)color.R << 16) | ((uint)color.G << 8) | (uint)color.B;
        }
    }
}
