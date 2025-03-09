using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs
{
    public class OnPlayerCashUpdateEventArgs : ServerEventArgs
    {
        public Player? Player { get; set; }
        public ulong CurrencyId { get; set; }
    }
}
