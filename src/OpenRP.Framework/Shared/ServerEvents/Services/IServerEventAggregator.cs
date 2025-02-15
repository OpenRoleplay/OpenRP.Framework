using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.ServerEvents.Services
{
    public interface IServerEventAggregator
    {
        void Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : ServerEventArgs;
        Task PublishAsync<TEvent>(TEvent eventArgs) where TEvent : ServerEventArgs;
    }
}
