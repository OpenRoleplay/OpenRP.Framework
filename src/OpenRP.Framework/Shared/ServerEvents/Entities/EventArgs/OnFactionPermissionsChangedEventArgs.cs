using System.Collections.Generic;

namespace OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs
{
    /// <summary>
    /// The permissions that come from a faction may have changed for these characters, for example because a grant
    /// changed or someone joined, left or got another rank. Their command permissions are reloaded on this.
    /// </summary>
    public class OnFactionPermissionsChangedEventArgs : ServerEventArgs
    {
        public ulong FactionId { get; set; }
        public List<ulong> CharacterIds { get; set; } = new List<ulong>();
    }
}
