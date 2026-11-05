using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.ServerEvents.Services
{
    public interface IServerEventDispatcher
    {
        /// <summary>
        /// Runs every [ServerEvent] handler whose event parameter accepts these event args, one after another.
        /// </summary>
        Task DispatchAsync(ServerEventArgs eventArgs);
    }
}
