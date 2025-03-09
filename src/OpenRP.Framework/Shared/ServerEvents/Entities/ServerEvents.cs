using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.ServerEvents.Entities
{
    public static class ServerEvents
    {
        // Dictionary mapping event names to their parameter types
        public static readonly Dictionary<string, Type> EventParameterTypes = new Dictionary<string, Type>
        {
            { "OnAccountLoggedIn", typeof(OnAccountLoggedInEventArgs) },
            { "OnPlayerCashUpdate", typeof(OnPlayerCashUpdateEventArgs) },
            { "OnCharacterSelected", typeof(OnCharacterSelectedEventArgs) },
            { "OnPlayerInventoryItemUsed", typeof(OnPlayerInventoryItemUsedEventArgs) }
            // Add more events and their parameter types here
        };
    }
}
