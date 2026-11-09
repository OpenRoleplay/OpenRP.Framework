namespace OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs
{
    /// <summary>
    /// The jurisdictions were loaded at startup. Things that show a jurisdiction, such as licence plates, update
    /// themselves in case they were created before the jurisdictions were there.
    /// </summary>
    public class OnJurisdictionsLoadedEventArgs : ServerEventArgs
    {
    }
}
