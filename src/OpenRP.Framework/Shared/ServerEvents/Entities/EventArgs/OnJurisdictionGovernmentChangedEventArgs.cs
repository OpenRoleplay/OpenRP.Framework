namespace OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs
{
    /// <summary>
    /// A jurisdiction got a new government of its own: a coup, an election or an admin's change. The faction ids are
    /// null for none, in which case the jurisdiction falls back to its parent's government.
    /// </summary>
    public class OnJurisdictionGovernmentChangedEventArgs : ServerEventArgs
    {
        public ulong JurisdictionId { get; set; }
        public ulong? OldGovernmentFactionId { get; set; }
        public ulong? NewGovernmentFactionId { get; set; }
    }
}
