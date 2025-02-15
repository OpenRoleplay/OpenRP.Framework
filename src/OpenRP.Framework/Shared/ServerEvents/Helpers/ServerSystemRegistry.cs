using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.ServerEvents.Helpers
{
    public static class ServerSystemRegistry
    {
        public static List<Assembly> RegisteredAssemblies { get; } = new List<Assembly>();
    }
}
