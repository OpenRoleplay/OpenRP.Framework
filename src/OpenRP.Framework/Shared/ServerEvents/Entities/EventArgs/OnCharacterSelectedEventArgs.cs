using OpenRP.Framework.Features.Accounts.Components;
using OpenRP.Framework.Features.Characters.Components;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs
{
    public class OnCharacterSelectedEventArgs : ServerEventArgs
    {
        public Player? Player { get; set; }
        public Account? Account { get; set; }
        public Character? Character { get; set; }
    }
}
