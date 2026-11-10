namespace OpenRP.Framework.Features.Phones.Entities
{
    /// <summary>
    /// A number below 1000 that the server answers itself, such as 911. Register one with
    /// AddServerPhoneNumber&lt;T&gt;(); a gamemode can add its own, for example a taxi company.
    /// </summary>
    public interface IServerPhoneNumber
    {
        /// <summary>
        /// The number, below 1000.
        /// </summary>
        uint GetNumber();

        /// <summary>
        /// What the caller's phone shows, for example "Emergency".
        /// </summary>
        string GetName();

        /// <summary>
        /// The number picked up. Greet the caller here.
        /// </summary>
        void OnCallConnected(IServerPhoneLine line);

        /// <summary>
        /// The caller said something into the phone.
        /// </summary>
        void OnCallerSpoke(IServerPhoneLine line, string text);

        /// <summary>
        /// The call ended, by either side. Forget anything kept for it.
        /// </summary>
        void OnCallEnded(IServerPhoneLine line);
    }
}
