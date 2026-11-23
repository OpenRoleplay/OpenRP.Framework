namespace OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs
{
    /// <summary>
    /// An admin deleted a saved vehicle. Its row is already gone; things fitted to it, such as a radio or a fishing net,
    /// clean up what the database cascade doesn't reach.
    /// </summary>
    public class OnVehicleDeletedEventArgs : ServerEventArgs
    {
        public ulong VehicleId { get; set; }
    }
}
