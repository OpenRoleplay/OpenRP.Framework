using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database;
using OpenRP.Framework.Features.Accounts.Components;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Accounts.Services
{
    /// <summary>
    /// Uses single update statements, so the database decides when two servers log the same account in at once, and
    /// nothing is tracked by the long-lived database context.
    /// </summary>
    public class AccountSessionService : IAccountSessionService
    {
        private static readonly TimeSpan SessionTimeout = TimeSpan.FromMinutes(2);

        private readonly BaseDataContext _dataContext;
        private readonly IEntityManager _entityManager;

        public AccountSessionService(BaseDataContext dataContext, IEntityManager entityManager)
        {
            _dataContext = dataContext;
            _entityManager = entityManager;
        }

        public bool TryClaimSession(ulong accountId)
        {
            DateTime now = DateTime.UtcNow;
            DateTime expiredBefore = now - SessionTimeout;

            int claimedAccounts = _dataContext.Accounts
                .Where(account => account.Id == accountId && (!account.IsOnline || account.LastSeenAt == null || account.LastSeenAt < expiredBefore))
                .ExecuteUpdate(setters => setters
                    .SetProperty(account => account.IsOnline, true)
                    .SetProperty(account => account.LastSeenAt, now));

            return claimedAccounts == 1;
        }

        public void ReleaseSession(ulong accountId)
        {
            ReleaseSessions(new List<ulong> { accountId });
        }

        public void RefreshSessions()
        {
            List<ulong> accountIds = GetLoggedInAccountIds();
            if (accountIds.Count == 0)
            {
                return;
            }

            DateTime now = DateTime.UtcNow;
            _dataContext.Accounts
                .Where(account => accountIds.Contains(account.Id))
                .ExecuteUpdate(setters => setters.SetProperty(account => account.LastSeenAt, now));
        }

        public void ReleaseAllSessions()
        {
            ReleaseSessions(GetLoggedInAccountIds());
        }

        private void ReleaseSessions(List<ulong> accountIds)
        {
            if (accountIds.Count == 0)
            {
                return;
            }

            _dataContext.Accounts
                .Where(account => accountIds.Contains(account.Id))
                .ExecuteUpdate(setters => setters.SetProperty(account => account.IsOnline, false));
        }

        private List<ulong> GetLoggedInAccountIds()
        {
            return _entityManager.GetComponents<Account>().Select(account => account.GetAccountId()).Distinct().ToList();
        }
    }
}
