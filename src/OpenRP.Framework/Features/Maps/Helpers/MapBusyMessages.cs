using OpenRP.Framework.Features.Maps.Enums;

namespace OpenRP.Framework.Features.Maps.Helpers
{
    /// <summary>
    /// The message that tells an admin why a map can't be changed right now.
    /// </summary>
    public static class MapBusyMessages
    {
        public static string For(MapBusyReason reason)
        {
            return reason switch
            {
                MapBusyReason.Regenerating => "This map is being regenerated.",
                MapBusyReason.Deleting => "This map is being deleted.",
                MapBusyReason.LockedAfterFailure => "This map is locked after a failed regeneration. Restart the server first.",
                _ => throw new System.ArgumentOutOfRangeException(nameof(reason), reason, null)
            };
        }
    }
}
