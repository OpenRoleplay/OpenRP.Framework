using System;

namespace OpenRP.Framework.Features.Discord.Entities
{
    /// <summary>
    /// A channel waiting to get a new name. QueuedAt decides its place in line.
    /// </summary>
    public class DiscordChannelRename
    {
        public ulong ChannelId { get; }
        public string Name { get; set; }
        public DateTime QueuedAt { get; }

        public DiscordChannelRename(ulong channelId, string name, DateTime queuedAt)
        {
            ChannelId = channelId;
            Name = name;
            QueuedAt = queuedAt;
        }
    }
}
