using System;
using System.Reflection;

namespace OpenRP.Framework.Shared.ServerEvents.Entities
{
    /// <summary>
    /// One [ServerEvent] method of an IServerSystem, and the event (args type) it handles.
    /// </summary>
    internal sealed record ServerEventHandler(object System, MethodInfo Method, ParameterInfo[] Parameters, Type EventType)
    {
        public string GetName()
        {
            return $"{Method.DeclaringType?.Name}.{Method.Name}";
        }
    }
}
