using OpenRP.Framework.Features.EmergencyCalls.Entities;
using OpenRP.Framework.Features.EmergencyCalls.Enums;
using OpenRP.Framework.Features.Permissions.Components;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Helpers;
using OpenRP.Framework.Shared.Chat.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.EmergencyCalls.Services
{
    public class DispatchService : IDispatchService
    {
        private const string RadioDoText = "Their radio crackles as dispatch comes through.";
        private static readonly Color RadioColor = new Color(130, 200, 160);

        private readonly IEntityManager _entityManager;
        private readonly IChatService _chatService;

        public DispatchService(IEntityManager entityManager, IChatService chatService)
        {
            _entityManager = entityManager;
            _chatService = chatService;
        }

        public void Broadcast(EmergencyServiceType service, string text)
        {
            List<string> radioLines = ChatLineSplitter.Split($"[Radio] Dispatch ({GetServiceName(service)}): {text}");
            foreach (Player listener in GetListeners())
            {
                _chatService.SendPlayerChatMessage(listener, PlayerChatMessageType.DO, RadioDoText);
                foreach (string radioLine in radioLines)
                {
                    listener.SendClientMessage(RadioColor, radioLine);
                }
            }
        }

        private IEnumerable<Player> GetListeners()
        {
            return _entityManager.GetComponents<Player>().Where(CanHearDispatch).ToList();
        }

        private static bool CanHearDispatch(Player player)
        {
            if (!player.IsPlayerPlayingAsCharacter())
            {
                return false;
            }
            CharacterPermissions? permissions = player.GetPlayerCurrentlyPlayingAsCharacter().GetComponent<CharacterPermissions>();
            return permissions != null && permissions.DoesCharacterHavePermission(DispatchPermissions.Receive);
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
