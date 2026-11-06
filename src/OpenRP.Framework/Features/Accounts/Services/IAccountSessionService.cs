namespace OpenRP.Framework.Features.Accounts.Services
{
    /// <summary>
    /// Makes sure an account is logged in only once at a time, on any server sharing the database. A server confirms its
    /// logged-in accounts every 30 seconds; a login that hasn't been confirmed for 2 minutes (a crashed server) expires.
    /// </summary>
    public interface IAccountSessionService
    {
        /// <summary>
        /// Marks the account as online when it isn't already. False when it's logged in somewhere.
        /// </summary>
        bool TryClaimSession(ulong accountId);

        void ReleaseSession(ulong accountId);

        /// <summary>
        /// Confirms that the accounts logged in on this server are still online. Called on a timer.
        /// </summary>
        void RefreshSessions();

        /// <summary>
        /// Marks every account logged in on this server as offline. Called on shutdown.
        /// </summary>
        void ReleaseAllSessions();
    }
}
