using System;

namespace OpenRP.Framework.Shared.ServerEvents.Attributes
{
    /// <summary>
    /// Marks a method of an IServerSystem as a server event handler. The event it handles is the type of its
    /// ServerEventArgs parameter; by convention the method is named after the event (On&lt;Event&gt;).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ServerEventAttribute : Attribute
    {
    }
}
