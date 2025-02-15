using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.ServerEvents.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ServerEventAttribute : Attribute
    {
        // Optionally, you can specify the event name explicitly
        // If not provided, the method name will be used as the event name
        public string EventName { get; }

        public ServerEventAttribute()
        {
        }

        public ServerEventAttribute(string eventName)
        {
            EventName = eventName;
        }
    }
}
