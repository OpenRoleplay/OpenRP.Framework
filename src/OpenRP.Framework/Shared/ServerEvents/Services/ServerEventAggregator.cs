using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using System;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.ServerEvents.Services
{
    public class ServerEventAggregator : IServerEventAggregator
    {
        // Resolved on first publish, not in the constructor: building the dispatcher creates the server systems,
        // and their services (for example AccountService) take this aggregator, which would be a circular dependency.
        private readonly Lazy<IServerEventDispatcher> _serverEventDispatcher;

        public ServerEventAggregator(IServiceProvider serviceProvider)
        {
            _serverEventDispatcher = new Lazy<IServerEventDispatcher>(() => serviceProvider.GetRequiredService<IServerEventDispatcher>());
        }

        public Task PublishAsync<TEvent>(TEvent eventArgs) where TEvent : ServerEventArgs
        {
            return _serverEventDispatcher.Value.DispatchAsync(eventArgs);
        }
    }
}
