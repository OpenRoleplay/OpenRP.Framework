using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Features.Maps.Entities;
using OpenRP.Framework.Features.Maps.Enums;
using System.Globalization;

namespace OpenRP.Framework.Features.Maps.Helpers
{
    /// <summary>
    /// The text the map dialogs show for objects, materials, removed buildings and positions.
    /// </summary>
    public static class MapDialogText
    {
        private const string NoValue = "-";

        /// <summary>
        /// The object's id, model and description, as a row of the object lists.
        /// </summary>
        public static string GetObjectLabel(MapObject mapObject)
        {
            return $"#{mapObject.GetId()}  {mapObject.GetModelId()}  {GetDescriptionOrDash(mapObject)}";
        }

        public static string GetObjectTitle(MapObject mapObject)
        {
            return $"Object #{mapObject.GetId()} ({mapObject.GetModelId()})";
        }

        public static string GetDescriptionOrDash(MapObject mapObject)
        {
            return mapObject.GetDescription() ?? NoValue;
        }

        public static string GetWorldLabel(MapObject mapObject)
        {
            return $"World: {mapObject.GetVirtualWorld()}  Interior: {mapObject.GetInterior()}";
        }

        /// <summary>
        /// The stream distance in metres, marked as the default when the object has none of its own.
        /// </summary>
        public static string GetStreamDistanceText(MapObject mapObject)
        {
            float? streamDistance = mapObject.GetStreamDistanceSetting();
            return streamDistance == null
                ? $"{FormatNumber(MapLimits.DefaultStreamDistance)} (default)"
                : FormatNumber(streamDistance.Value);
        }

        /// <summary>
        /// What the material slot shows, or a dash when nothing is set there.
        /// </summary>
        public static string GetMaterialLabel(MapObject mapObject, int materialIndex)
        {
            MapObjectMaterial? material = mapObject.GetMaterial(materialIndex);
            return material == null ? NoValue : GetMaterialContent(material);
        }

        public static string GetMaterialContent(MapObjectMaterial material)
        {
            if (material.GetMaterialType() == MapObjectMaterialType.Text)
            {
                return $"Text \"{material.GetText()}\"";
            }
            return $"Texture {material.GetTextureModelId()}/{material.GetTxdName()}/{material.GetTextureName()}";
        }

        public static string GetRemovedBuildingLabel(MapRemovedBuilding removal)
        {
            return $"{removal.GetModelId()}  {FormatPosition(removal.GetPosition())}  {FormatNumber(removal.GetRadius())}";
        }

        public static string FormatPosition(Vector3 position)
        {
            return $"{FormatNumber(position.X)} {FormatNumber(position.Y)} {FormatNumber(position.Z)}";
        }

        public static string GetObjectCountText(int count)
        {
            return count == 1 ? "1 object" : $"{count} objects";
        }

        private static string FormatNumber(float value)
        {
            return value.ToString("0.##", CultureInfo.InvariantCulture);
        }
    }
}
