using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.ServerEvents.Services
{
    public interface IServerEventAggregator
    {
        /// <summary>
        /// Publishes the event to every [ServerEvent] handler that takes these event args, and waits for all of them.
        /// </summary>
        Task PublishAsync<TEvent>(TEvent eventArgs) where TEvent : ServerEventArgs;
    }
}
