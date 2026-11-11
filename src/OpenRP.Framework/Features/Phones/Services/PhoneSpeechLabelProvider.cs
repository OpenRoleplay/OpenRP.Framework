using OpenRP.Framework.Features.Phones.Components;
using OpenRP.Framework.Shared.Chat.Entities;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System.Linq;

namespace OpenRP.Framework.Features.Phones.Services
{
    /// <summary>
    /// Labels speech "(phone)" while the player is in a connected call, so people nearby know who they're talking to.
    /// Reads the call components directly: the call service depends on the chat, which asks this provider.
    /// </summary>
    public class PhoneSpeechLabelProvider : IChatSpeechLabelProvider
    {
        private const string PhoneLabel = "phone";

        private readonly IEntityManager _entityManager;

        public PhoneSpeechLabelProvider(IEntityManager entityManager)
        {
            _entityManager = entityManager;
        }

        public string? GetSpeechLabel(Player player)
        {
            return IsInConnectedCall(player) ? PhoneLabel : null;
        }

        private bool IsInConnectedCall(Player player)
        {
            return _entityManager.GetComponents<PhoneCall>()
                .Any(call => call.IsConnected() && (call.GetCallerPlayer() == player.Entity || call.GetCalledPlayer() == player.Entity));
        }
    }
}
