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
        /// How far, in metres, an object is streamed in when it has no distance of its own.
        /// </summary>
        public const float DefaultStreamDistance = 300f;

        /// <summary>
        /// The shortest and the longest stream distance an object can be given, in metres.
        /// </summary>
        public const float MinStreamDistance = 1f;

        public const float MaxStreamDistance = 1000f;

        /// <summary>
        /// The virtual world and interior that mean "everywhere". Nothing below it is a world or interior.
        /// </summary>
        public const int EverywhereValue = -1;
    }
}
