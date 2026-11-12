using OpenRP.Framework.Database;
using System;

namespace OpenRP.Framework.Features.Banking.Services
{
    public interface IBankingDatabase
    {
        /// <summary>
        /// Runs the work with a fresh database context and returns its result.
        /// </summary>
        T Use<T>(Func<BaseDataContext, T> work);

        /// <summary>
        /// Runs the work with a fresh database context.
        /// </summary>
        void Use(Action<BaseDataContext> work);

        /// <summary>
        /// Runs the work in a database transaction with a fresh context, committing only when it returns true. A
        /// dropped connection retries the whole transaction, so the work must start from scratch each time.
        /// </summary>
        bool UseInTransaction(Func<BaseDataContext, bool> work);
    }
}
