namespace OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs
{
    /// <summary>
    /// An admin deleted a property. Things standing inside it, such as actors, react by moving outside.
    /// </summary>
    public class OnPropertyDeletedEventArgs : ServerEventArgs
    {
        public ulong PropertyId { get; set; }
    }
}
