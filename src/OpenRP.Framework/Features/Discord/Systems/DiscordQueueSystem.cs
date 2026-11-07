using OpenRP.Framework.Features.Discord.Services;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Discord.Systems
{
    public class DiscordQueueSystem : ISystem
    {
        // Often enough that a waiting rename goes soon after its channel's cooldown ends.
        private const int RenameCheckIntervalInMilliseconds = 15000;

        [Timer(RenameCheckIntervalInMilliseconds)]
        public void SendNextChannelRename(IDiscordService discordService)
        {
            // Logs its own errors, so nothing is lost by not awaiting it here.
            _ = discordService.SendNextChannelRenameAsync();
        }
    }
}
