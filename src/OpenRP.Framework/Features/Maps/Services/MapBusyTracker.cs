using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Features.Maps.Enums;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Maps.Services
{
    public class MapBusyTracker : IMapBusyTracker
    {
        private readonly Dictionary<ulong, MapBusyReason> _reasonsByMapId = new Dictionary<ulong, MapBusyReason>();

        public MapBusyReason? GetReason(Map? map)
        {
            if (map == null)
            {
                return null;
            }
            if (_reasonsByMapId.TryGetValue(map.GetId(), out MapBusyReason reason))
            {
                return reason;
            }
            return null;
        }

        public void Set(Map map, MapBusyReason reason)
        {
            _reasonsByMapId[map.GetId()] = reason;
        }

        public void Clear(Map map)
        {
            _reasonsByMapId.Remove(map.GetId());
        }
    }
}
