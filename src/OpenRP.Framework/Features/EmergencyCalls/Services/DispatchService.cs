using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenRP.Framework.Features.EmergencyCalls.Entities;
using OpenRP.Framework.Features.EmergencyCalls.Enums;
using OpenRP.Framework.Features.Radios.Enums;
using OpenRP.Framework.Features.Radios.Services;

namespace OpenRP.Framework.Features.EmergencyCalls.Services
{
    public class DispatchService : IDispatchService
    {
        private const string DispatchFrequencyName = "911 Dispatch";

        private readonly IRadioBroadcastService _radioBroadcastService;
        private readonly IRadioEditorService _radioEditorService;
        private readonly EmergencyCallOptions _options;
        private readonly ILogger<DispatchService> _logger;

        public DispatchService(IRadioBroadcastService radioBroadcastService, IRadioEditorService radioEditorService, IOptions<EmergencyCallOptions> options,
            ILogger<DispatchService> logger)
        {
            _radioBroadcastService = radioBroadcastService;
            _radioEditorService = radioEditorService;
            _options = options.Value;
            _logger = logger;
        }

        public void Broadcast(EmergencyServiceType service, string text)
        {
            _radioBroadcastService.TransmitFromSystem(_options.DispatchFrequency, $"Dispatch ({GetServiceName(service)})", text);
        }

        public void CreateDispatchFrequencyIfMissing()
        {
            if (_radioEditorService.CreateFrequencyIfNotExists(_options.DispatchFrequency, DispatchFrequencyName, RadioFrequencyMode.Protected) == null)
            {
                _logger.LogWarning("The dispatch frequency {Frequency} couldn't be created; 911 broadcasts follow its default rules.", _options.DispatchFrequency);
            }
        }

        private static string GetServiceName(EmergencyServiceType service)
        {
            return service switch
            {
                EmergencyServiceType.Fire => "Fire",
                EmergencyServiceType.Ambulance => "EMS",
                _ => "Police"
            };
        }
    }
}
