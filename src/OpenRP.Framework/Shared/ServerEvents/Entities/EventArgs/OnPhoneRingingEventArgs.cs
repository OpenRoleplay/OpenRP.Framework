using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs
{
    /// <summary>
    /// A call started ringing on a phone someone carries. The phone feature opens the answer dialog for the carrier.
    /// </summary>
    public class OnPhoneRingingEventArgs : ServerEventArgs
    {
        public Player? Carrier { get; set; }
        public ulong PhoneId { get; set; }
        public uint CallerNumber { get; set; }
    }
}
