using OpenRP.Framework.Features.Discord.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Discord.Services
{
    public class DiscordChannelNameQueue : IDiscordChannelNameQueue
    {
        /// <summary>
        /// Discord allows 2 renames per channel per 10 minutes.
        /// </summary>
        public static readonly TimeSpan RenameCooldown = TimeSpan.FromMinutes(5);

        private readonly List<DiscordChannelRename> _waitingRenames = new List<DiscordChannelRename>();
        private readonly Dictionary<ulong, string> _currentNames = new Dictionary<ulong, string>();
        private readonly Dictionary<ulong, DateTime> _lastRenamedAt = new Dictionary<ulong, DateTime>();
        private DiscordChannelRename? _renameInProgress;

        public void SetChannelName(ulong channelId, string name, DateTime now)
        {
            DiscordChannelRename? waitingRename = GetWaitingRename(channelId);
            if (HasCurrentName(channelId, name))
            {
                RemoveWaitingRename(waitingRename);
                return;
            }

            if (waitingRename != null)
            {
                waitingRename.Name = name;
                return;
            }
            _waitingRenames.Add(new DiscordChannelRename(channelId, name, now));
        }

        public DiscordChannelRename? TakeNextRename(DateTime now)
        {
            if (_renameInProgress != null)
            {
                return null;
            }

            DiscordChannelRename? nextRename = _waitingRenames
                .Where(rename => !IsCoolingDown(rename.ChannelId, now))
                .OrderBy(rename => rename.QueuedAt)
                .FirstOrDefault();
            if (nextRename == null)
            {
                return null;
            }

            _waitingRenames.Remove(nextRename);
            _renameInProgress = nextRename;
            return nextRename;
        }

        public void MarkRenamed(DiscordChannelRename rename, DateTime? renamedAt)
        {
            _renameInProgress = null;
            _currentNames[rename.ChannelId] = rename.Name;
            if (renamedAt.HasValue)
            {
                _lastRenamedAt[rename.ChannelId] = renamedAt.Value;
            }

            // A name queued while this rename ran may turn out to be the one the channel now has.
            DiscordChannelRename? waitingRename = GetWaitingRename(rename.ChannelId);
            if (waitingRename != null && waitingRename.Name == rename.Name)
            {
                RemoveWaitingRename(waitingRename);
            }
        }

        public void MarkRenameFailed(DiscordChannelRename rename)
        {
            _renameInProgress = null;
            if (GetWaitingRename(rename.ChannelId) == null)
            {
                _waitingRenames.Add(rename);
            }
        }

        private DiscordChannelRename? GetWaitingRename(ulong channelId)
        {
            return _waitingRenames.FirstOrDefault(rename => rename.ChannelId == channelId);
        }

        private void RemoveWaitingRename(DiscordChannelRename? waitingRename)
        {
            if (waitingRename != null)
            {
                _waitingRenames.Remove(waitingRename);
            }
        }

        private bool HasCurrentName(ulong channelId, string name)
        {
            return _currentNames.TryGetValue(channelId, out string? currentName) && currentName == name;
        }

        private bool IsCoolingDown(ulong channelId, DateTime now)
        {
            return _lastRenamedAt.TryGetValue(channelId, out DateTime lastRenamedAt) && now - lastRenamedAt < RenameCooldown;
        }
    }
}
