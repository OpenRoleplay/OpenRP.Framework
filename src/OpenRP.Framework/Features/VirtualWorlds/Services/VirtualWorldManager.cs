using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.VirtualWorlds.Services
{
    public class VirtualWorldManager : IVirtualWorldManager
    {
        private int _nextVirtualWorld;
        private Dictionary<string, int> _mappingVirtualWorlds;

        public VirtualWorldManager()
        {
            _nextVirtualWorld = 1;
            _mappingVirtualWorlds = new Dictionary<string, int>();
        }

        private int ClaimNextAvailableVirtualWorld(string key)
        {
            int claimedVirtualWorld = _nextVirtualWorld;
            _mappingVirtualWorlds.Add(key, claimedVirtualWorld);
            _nextVirtualWorld++;
            return claimedVirtualWorld;
        }

        public int GetMainMenuSceneVirtualWorld(ulong id)
        {
            string vwKey = $"MMS_{id}";

            if (_mappingVirtualWorlds.TryGetValue(vwKey, out int vw))
            {
                return vw;
            }

            return ClaimNextAvailableVirtualWorld(vwKey);
        }

        public int GetPropertyVirtualWorld(ulong id)
        {
            string vwKey = $"PROPERTY_{id}";

            if (_mappingVirtualWorlds.TryGetValue(vwKey, out int vw))
            {
                return vw;
            }

            return ClaimNextAvailableVirtualWorld(vwKey);
        }
    }
}
