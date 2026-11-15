using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Features.Maps.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Maps.Helpers
{
    /// <summary>
    /// Checks the objects of a bulk replace before anything is written, so one bad object refuses the whole replace.
    /// </summary>
    public static class MapBulkValidator
    {
        private const int MaxTextureNameLength = 64;

        /// <summary>
        /// The first problem found, naming the object, or null when every object can be saved.
        /// </summary>
        public static string? GetObjectsError(IReadOnlyList<MapObjectSpec> objects)
        {
            for (int index = 0; index < objects.Count; index++)
            {
                string? error = GetObjectError(objects[index]);
                if (error != null)
                {
                    return $"Object {index + 1}: {error}";
                }
            }
            return null;
        }

        private static string? GetObjectError(MapObjectSpec spec)
        {
            return GetModelError(spec.ModelId)
                ?? GetPlacementError(spec)
                ?? GetWorldError(spec.VirtualWorld, spec.Interior)
                ?? GetStreamDistanceError(spec.StreamDistance)
                ?? GetDescriptionError(spec.Description)
                ?? GetTexturesError(spec.Textures);
        }

        private static string? GetModelError(int modelId)
        {
            return modelId == 0 ? "The model ID can't be 0. Negative IDs are custom models." : null;
        }

        private static string? GetPlacementError(MapObjectSpec spec)
        {
            return IsFinite(spec.Position) && IsFinite(spec.Rotation) ? null : "The position and rotation must be finite numbers.";
        }

        private static string? GetWorldError(int virtualWorld, int interior)
        {
            return virtualWorld >= MapLimits.EverywhereValue && interior >= MapLimits.EverywhereValue
                ? null
                : $"The virtual world and interior can't be below {MapLimits.EverywhereValue}; {MapLimits.EverywhereValue} means everywhere.";
        }

        /// <summary>
        /// The comparisons are written so that NaN is refused too.
        /// </summary>
        private static string? GetStreamDistanceError(float? streamDistance)
        {
            if (streamDistance == null)
            {
                return null;
            }

            float distance = streamDistance.Value;
            return distance >= MapLimits.MinStreamDistance && distance <= MapLimits.MaxStreamDistance
                ? null
                : $"The stream distance must be from {MapLimits.MinStreamDistance} to {MapLimits.MaxStreamDistance} metres.";
        }

        private static string? GetDescriptionError(string? description)
        {
            return description?.Length > MapLimits.DescriptionMaxLength
                ? $"The description can be at most {MapLimits.DescriptionMaxLength} characters long."
                : null;
        }

        private static string? GetTexturesError(IReadOnlyList<MapObjectTextureSpec>? textures)
        {
            if (textures == null)
            {
                return null;
            }
            // The database allows one material per slot, so a duplicate would fail the whole transaction.
            if (textures.Select(texture => texture.MaterialIndex).Distinct().Count() != textures.Count)
            {
                return "Two textures of one object use the same material index.";
            }
            foreach (MapObjectTextureSpec texture in textures)
            {
                string? error = GetTextureError(texture);
                if (error != null)
                {
                    return error;
                }
            }
            return null;
        }

        private static string? GetTextureError(MapObjectTextureSpec texture)
        {
            if (texture.MaterialIndex is < 0 or > MapObjectMaterial.MaxMaterialIndex)
            {
                return $"The material index must be from 0 to {MapObjectMaterial.MaxMaterialIndex}.";
            }
            if (texture.ModelId == 0)
            {
                return "The texture's model ID can't be 0.";
            }
            return IsValidName(texture.TxdName) && IsValidName(texture.TextureName)
                ? null
                : $"The TXD and texture names must be 1 to {MaxTextureNameLength} characters long.";
        }

        private static bool IsValidName(string? name)
        {
            return name is { Length: > 0 and <= MaxTextureNameLength };
        }

        private static bool IsFinite(System.Numerics.Vector3 vector)
        {
            return float.IsFinite(vector.X) && float.IsFinite(vector.Y) && float.IsFinite(vector.Z);
        }
    }
}
