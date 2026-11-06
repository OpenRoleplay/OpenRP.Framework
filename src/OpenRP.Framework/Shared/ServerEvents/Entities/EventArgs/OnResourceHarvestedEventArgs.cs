using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs
{
    /// <summary>
    /// A player finished harvesting a resource, for example "hemp". Rewards and skill experience react to this.
    /// </summary>
    public class OnResourceHarvestedEventArgs : ServerEventArgs
    {
        public Player? Player { get; set; }
        public string ResourceName { get; set; } = string.Empty;
    }
}
