using OpenRP.Framework.Features.EmergencyCalls.Components;
using OpenRP.Framework.Features.EmergencyCalls.Entities;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.EmergencyCalls.Services
{
    /// <summary>
    /// The AI that plays the 911 dispatcher.
    /// </summary>
    public interface IEmergencyDispatcherAi
    {
        /// <summary>
        /// Whether an API key is set. Without one, callers get a form instead.
        /// </summary>
        bool IsConfigured();

        /// <summary>
        /// The dispatcher's next lines, given everything said in the call so far.
        /// </summary>
        Task<EmergencyDispatcherReply> GenerateReplyAsync(EmergencyCall call);
    }
}
