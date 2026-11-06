using OpenRP.Framework.Features.Accounts.Components;
using OpenRP.Framework.Features.Accounts.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Accounts.Systems
{
    public class AccountSessionSystem : ISystem
    {
        // Must stay well below the session timeout in AccountSessionService (2 minutes).
        private const int SessionRefreshIntervalInMilliseconds = 30000;

        [Event]
        public void OnPlayerDisconnect(Player player, DisconnectReason disconnectReason, IAccountSessionService accountSessionService)
        {
            Account? account = player.GetComponent<Account>();
            if (account != null)
            {
                accountSessionService.ReleaseSession(account.GetAccountId());
            }
        }

        [Timer(SessionRefreshIntervalInMilliseconds)]
        public void RefreshSessions(IAccountSessionService accountSessionService)
        {
            accountSessionService.RefreshSessions();
        }

        [Event]
        public void OnGameModeExit(IAccountSessionService accountSessionService)
        {
            accountSessionService.ReleaseAllSessions();
        }
    }
}
