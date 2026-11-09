namespace OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs
{
    /// <summary>
    /// An admin deleted a jurisdiction. Things that lay in it, such as properties and vehicles, react by having none.
    /// </summary>
    public class OnJurisdictionDeletedEventArgs : ServerEventArgs
    {
        public ulong JurisdictionId { get; set; }
    }
}
