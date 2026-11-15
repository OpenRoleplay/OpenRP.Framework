namespace OpenRP.Framework.Features.Maps.Enums
{
    /// <summary>
    /// Why a map refuses changes. A map is busy while its objects are replaced or while it's deleted. After a replace whose
    /// rows were saved but whose objects couldn't all be put in the game, it stays locked until the server restarts.
    /// </summary>
    public enum MapBusyReason
    {
        Regenerating,
        Deleting,
        LockedAfterFailure
    }
}
