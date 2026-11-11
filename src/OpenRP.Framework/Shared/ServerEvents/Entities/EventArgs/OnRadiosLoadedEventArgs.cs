namespace OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs
{
    /// <summary>
    /// The radios and frequencies were loaded at startup. Features that need a frequency to exist, such as 911 dispatch,
    /// create it now.
    /// </summary>
    public class OnRadiosLoadedEventArgs : ServerEventArgs
    {
    }
}
