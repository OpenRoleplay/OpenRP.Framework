using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.VirtualWorlds.Services
{
    public interface IVirtualWorldManager
    {
        int GetMainMenuSceneVirtualWorld(ulong id);
        int GetPropertyVirtualWorld(ulong id);

        /// <summary>
        /// The property that owns this virtual world, or null when it isn't a property's.
        /// </summary>
        ulong? GetPropertyIdOfVirtualWorld(int virtualWorld);
    }
}
