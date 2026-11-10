namespace OpenRP.Framework.Features.EmergencyCalls.Entities
{
    public static class DispatchPermissions
    {
        /// <summary>
        /// Hears dispatch relay 911 calls. Until radios exist, this stands in for being on the dispatch channel.
        /// </summary>
        public const string Receive = "dispatch.receive";

        public const string ReceiveDescription = "Hear dispatch relay 911 calls on the radio.";
    }
}
