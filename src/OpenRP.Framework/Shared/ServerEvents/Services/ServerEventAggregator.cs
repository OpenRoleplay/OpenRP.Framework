using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.ServerEvents.Services
{
    public class ServerEventAggregator : IServerEventAggregator
    {
        private readonly ConcurrentDictionary<Type, List<Func<ServerEventArgs, Task>>> _handlers
            = new ConcurrentDictionary<Type, List<Func<ServerEventArgs, Task>>>();

        public void Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : ServerEventArgs
        {
            var eventType = typeof(TEvent);
            var wrappedHandler = new Func<ServerEventArgs, Task>(e => handler((TEvent)e));

            _handlers.AddOrUpdate(eventType,
                new List<Func<ServerEventArgs, Task>> { wrappedHandler },
                (type, existingHandlers) =>
                {
                    existingHandlers.Add(wrappedHandler);
                    return existingHandlers;
                });
        }

        public async Task PublishAsync<TEvent>(TEvent eventArgs) where TEvent : ServerEventArgs
        {
            var eventType = typeof(TEvent);
            if (_handlers.TryGetValue(eventType, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    await handler(eventArgs);
                }
            }
        }
    }
}
