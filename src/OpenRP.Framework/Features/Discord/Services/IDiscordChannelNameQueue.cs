using OpenRP.Framework.Features.Discord.Entities;
using System;

namespace OpenRP.Framework.Features.Discord.Services
{
    /// <summary>
    /// One line for all channel renames, so no channel is renamed more often than Discord allows. The channel waiting
    /// longest goes first, and a newer name for a waiting channel replaces the old one without losing its place.
    /// </summary>
    public interface IDiscordChannelNameQueue
    {
        /// <summary>
        /// Queues the channel to get this name, unless it already has it.
        /// </summary>
        void SetChannelName(ulong channelId, string name, DateTime now);

        /// <summary>
        /// Takes the rename that may go now, or null when none may: one runs at a time, and a channel waits
        /// <see cref="DiscordChannelNameQueue.RenameCooldown"/> after its last rename.
        /// </summary>
        DiscordChannelRename? TakeNextRename(DateTime now);

        /// <summary>
        /// Remembers the channel's new name. renamedAt is null when no rename was needed after all.
        /// </summary>
        void MarkRenamed(DiscordChannelRename rename, DateTime? renamedAt);

        /// <summary>
        /// Puts the rename back in line at its old place, unless a newer name for the channel is already waiting.
        /// </summary>
        void MarkRenameFailed(DiscordChannelRename rename);
    }
}
