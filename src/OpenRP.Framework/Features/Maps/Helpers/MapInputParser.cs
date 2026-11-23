using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Features.Maps.Enums;
using SampSharp.Entities.SAMP;
using System;
using System.Globalization;
using System.Linq;

namespace OpenRP.Framework.Features.Maps.Helpers
{
    /// <summary>
    /// Reads what admins type for textures, material text and removed buildings. Each parser returns the model to
    /// save, or the reason the input isn't valid. The models have no map or object id yet.
    /// </summary>
    public static class MapInputParser
    {
        public const float DefaultRemovedBuildingRadius = 0.25f;

        // Material text is always drawn on a 256 x 128 texture, the size the old server used.
        public const int MaterialTextSize = (int)ObjectMaterialSize.X256X128;

        private const int MaxNameLength = 64;
        private const int MaxTextLength = 256;
        private const int MaxAlignment = 2;
        private const int MaterialTextFieldCount = 7;
        private const int MaxArgbDigits = 8;

        private const string TextureUsage = "Use: model txd texture [ARGB colour], for example 3374 ce_farmxref sw_barnwood1 0x00000000. The model isn't 0, and the colour is hex, with or without 0x.";
        private const string MaterialTextUsage = "Use: text+index+font+size+bold+fontcolor+backcolor+alignment, for example Montgomery DMV+5+Arial+36+1+0xFF000000+0x00000000+1. "
            + "The index is 0 to 15, the size above 0, bold 0 or 1, the colours hex (ARGB), and the alignment 0 (left), 1 (centre) or 2 (right).";
        private const string MaterialTextFieldsError = "One of the fields after the text isn't valid. ";
        private const string RemovedBuildingUsage = "Use: model [radius] to remove a building around where you stand, or model x y z radius. "
            + "The model isn't 0, and the radius is above 0. Examples: 3374 or 3374 2.5, or 3374 1.5 2.0 10.0 5.";

        /// <summary>
        /// Reads "model txd texture [ARGB colour]". The colour is 0 when it's left out, which keeps the original colour.
        /// </summary>
        public static (MapObjectMaterialModel? Material, string? Error) ParseTexture(string input)
        {
            string[] words = SplitWords(input);
            if (words.Length is < 3 or > 4)
            {
                return (null, TextureUsage);
            }
            return TryCreateTexture(words);
        }

        /// <summary>
        /// Reads "text+index+font+size+bold+fontcolor+backcolor+alignment". The text may contain "+", since the last
        /// seven fields are read from the end. Every field is trimmed before it is read.
        /// </summary>
        public static (MapObjectMaterialModel? Material, string? Error) ParseMaterialText(string input)
        {
            string[] parts = input.Split('+');
            if (parts.Length <= MaterialTextFieldCount)
            {
                return (null, MaterialTextUsage);
            }

            int fieldStart = parts.Length - MaterialTextFieldCount;
            string text = string.Join("+", parts[..fieldStart]).Trim();
            string? textError = GetTextError(text);
            if (textError != null)
            {
                return (null, textError);
            }
            return TryCreateMaterialText(text, parts[fieldStart..].Select(field => field.Trim()).ToArray());
        }

        /// <summary>
        /// Reads "model [radius]", which removes the building around the origin, or "model x y z radius".
        /// </summary>
        public static (MapRemovedBuildingModel? Removal, string? Error) ParseRemovedBuilding(string input, Vector3 origin)
        {
            string[] words = SplitWords(input);
            if (words.Length is 1 or 2)
            {
                return TryCreateRemovedBuildingAtOrigin(words, origin);
            }
            if (words.Length == 5)
            {
                return TryCreateRemovedBuildingAtPosition(words);
            }
            return (null, RemovedBuildingUsage);
        }

        private static (MapObjectMaterialModel? Material, string? Error) TryCreateTexture(string[] words)
        {
            if (!MapDialogInput.TryParseModelId(words[0], out int modelId) || !IsValidName(words[1]) || !IsValidName(words[2]))
            {
                return (null, TextureUsage);
            }

            uint color = 0;
            if (words.Length == 4 && !TryParseArgb(words[3], out color))
            {
                return (null, TextureUsage);
            }

            return (new MapObjectMaterialModel
            {
                Type = MapObjectMaterialType.Texture,
                ModelId = modelId,
                TxdName = words[1],
                TextureName = words[2],
                Color = color
            }, null);
        }

        /// <summary>
        /// The fields are: index, font, size, bold, font colour, back colour and alignment.
        /// </summary>
        private static (MapObjectMaterialModel? Material, string? Error) TryCreateMaterialText(string text, string[] fields)
        {
            if (!TryParseMaterialIndex(fields[0], out int materialIndex)
                || !TryParseFontFace(fields[1], out string fontFace)
                || !TryParseFontSize(fields[2], out int fontSize)
                || !TryParseBold(fields[3], out bool bold)
                || !TryParseArgb(fields[4], out uint fontColor)
                || !TryParseArgb(fields[5], out uint backColor)
                || !TryParseAlignment(fields[6], out int alignment))
            {
                return (null, MaterialTextFieldsError + MaterialTextUsage);
            }

            return (new MapObjectMaterialModel
            {
                Type = MapObjectMaterialType.Text,
                MaterialIndex = materialIndex,
                Text = text,
                MaterialSize = MaterialTextSize,
                FontFace = fontFace,
                FontSize = fontSize,
                Bold = bold,
                FontColor = fontColor,
                BackColor = backColor,
                Alignment = alignment
            }, null);
        }

        private static (MapRemovedBuildingModel? Removal, string? Error) TryCreateRemovedBuildingAtOrigin(string[] words, Vector3 origin)
        {
            float radius = DefaultRemovedBuildingRadius;
            if (!MapDialogInput.TryParseModelId(words[0], out int modelId) || (words.Length == 2 && !TryParseRadius(words[1], out radius)))
            {
                return (null, RemovedBuildingUsage);
            }
            return (CreateRemovedBuilding(modelId, origin, radius), null);
        }

        private static (MapRemovedBuildingModel? Removal, string? Error) TryCreateRemovedBuildingAtPosition(string[] words)
        {
            if (!MapDialogInput.TryParseModelId(words[0], out int modelId)
                || !TryParseFloat(words[1], out float x)
                || !TryParseFloat(words[2], out float y)
                || !TryParseFloat(words[3], out float z)
                || !TryParseRadius(words[4], out float radius))
            {
                return (null, RemovedBuildingUsage);
            }
            return (CreateRemovedBuilding(modelId, new Vector3(x, y, z), radius), null);
        }

        private static MapRemovedBuildingModel CreateRemovedBuilding(int modelId, Vector3 position, float radius)
        {
            return new MapRemovedBuildingModel
            {
                ModelId = modelId,
                X = position.X,
                Y = position.Y,
                Z = position.Z,
                Radius = radius
            };
        }

        private static string[] SplitWords(string input)
        {
            return input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        private static string? GetTextError(string text)
        {
            if (text.Length == 0)
            {
                return "The text can't be empty. " + MaterialTextUsage;
            }
            return text.Length > MaxTextLength ? $"The text can be at most {MaxTextLength} characters long." : null;
        }

        private static bool IsValidName(string name)
        {
            return name.Length is > 0 and <= MaxNameLength;
        }

        private static bool TryParseFontFace(string text, out string fontFace)
        {
            fontFace = text.Trim();
            return IsValidName(fontFace);
        }

        private static bool TryParseMaterialIndex(string text, out int materialIndex)
        {
            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out materialIndex)
                && materialIndex is >= 0 and <= MapObjectMaterial.MaxMaterialIndex;
        }

        private static bool TryParseFontSize(string text, out int fontSize)
        {
            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out fontSize) && fontSize > 0;
        }

        private static bool TryParseBold(string text, out bool bold)
        {
            bold = text == "1";
            return text == "0" || text == "1";
        }

        private static bool TryParseAlignment(string text, out int alignment)
        {
            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out alignment)
                && alignment is >= 0 and <= MaxAlignment;
        }

        private static bool TryParseRadius(string text, out float radius)
        {
            return TryParseFloat(text, out radius) && radius > 0;
        }

        private static bool TryParseFloat(string text, out float value)
        {
            return float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) && float.IsFinite(value);
        }

        /// <summary>
        /// Reads a colour in ARGB hex, with or without the 0x prefix.
        /// </summary>
        private static bool TryParseArgb(string text, out uint argb)
        {
            string digits = text.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? text[2..] : text;
            if (digits.Length is < 1 or > MaxArgbDigits)
            {
                argb = 0;
                return false;
            }
            return uint.TryParse(digits, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out argb);
        }
    }
}
