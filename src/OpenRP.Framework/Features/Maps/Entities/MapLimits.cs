namespace OpenRP.Framework.Features.Maps.Entities
{
    /// <summary>
    /// The limits the map editor, its dialogs and the database configuration share.
    /// </summary>
    public static class MapLimits
    {
        public const int NameMaxLength = 64;

        public const int DescriptionMaxLength = 64;

        /// <summary>
        /// The virtual world and interior that mean "everywhere". Nothing below it is a world or interior.
        /// </summary>
        public const int EverywhereValue = -1;
    }
}
