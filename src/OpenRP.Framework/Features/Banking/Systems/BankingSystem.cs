using OpenRP.Framework.Features.Banking.Services;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Banking.Systems
{
    public class BankingSystem : ISystem
    {
        // Fees are due every 30 days, so checking once an hour is precise enough.
        private const int FeeIntervalInMilliseconds = 60 * 60 * 1000;

        [Timer(FeeIntervalInMilliseconds)]
        public void ChargeDueFees(IBankTransactionService bankTransactionService)
        {
            bankTransactionService.ChargeDueFees();
        }
    }
}
