using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Discord.Services;
using OpenRP.Framework.Shared.ServerEvents.Attributes;
using OpenRP.Framework.Shared.ServerEvents.Entities;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;

namespace OpenRP.Framework.Features.Discord.Systems
{
    public class DiscordServerSystem : IServerSystem
    {
        [ServerEvent]
        public void OnCharacterSelected(OnCharacterSelectedEventArgs args, IDiscordService discordService)
        {
            // Local test servers would otherwise announce on the live Discord.
#if !DEBUG
            if (discordService == null || args.Character == null)
            {
                return;
            }

            SendCharacterNowPlayingMessage(discordService, args.Character);
#endif
        }

        private static void SendCharacterNowPlayingMessage(IDiscordService discordService, Character character)
        {
            discordService.SendGeneralChatMessage($"## {character.GetName()} is now playing on the server.");
        }
    }
}
