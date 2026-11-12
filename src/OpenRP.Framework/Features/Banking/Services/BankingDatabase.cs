using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Database;
using System;

namespace OpenRP.Framework.Features.Banking.Services
{
    /// <summary>
    /// Gives every banking operation its own short-lived database context, so nothing stays tracked between operations
    /// and money is never saved from a stale copy.
    /// </summary>
    public class BankingDatabase : IBankingDatabase
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public BankingDatabase(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public T Use<T>(Func<BaseDataContext, T> work)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            return work(scope.ServiceProvider.GetRequiredService<BaseDataContext>());
        }

        public void Use(Action<BaseDataContext> work)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            work(scope.ServiceProvider.GetRequiredService<BaseDataContext>());
        }

        /// <summary>
        /// Through the execution strategy, because a host whose connection retries on errors refuses transactions
        /// opened directly. Each attempt clears what an earlier attempt added, so nothing is inserted twice.
        /// </summary>
        public bool UseInTransaction(Func<BaseDataContext, bool> work)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            BaseDataContext db = scope.ServiceProvider.GetRequiredService<BaseDataContext>();
            return db.Database.CreateExecutionStrategy().Execute(() =>
            {
                db.ChangeTracker.Clear();
                using IDbContextTransaction transaction = db.Database.BeginTransaction();
                if (!work(db))
                {
                    return false;
                }
                transaction.Commit();
                return true;
            });
        }
    }
}
