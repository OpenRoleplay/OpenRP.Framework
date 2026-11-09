namespace OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs
{
    /// <summary>
    /// The plate tag of a jurisdiction (its country's two-letter tag) changed: an admin changed the tags or moved it
    /// to another country. Vehicles registered there react by updating their plates.
    /// </summary>
    public class OnJurisdictionPlateTagChangedEventArgs : ServerEventArgs
    {
        public ulong JurisdictionId { get; set; }
    }
}
