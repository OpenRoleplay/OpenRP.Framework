using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.Factions.Components
{
    /// <summary>
    /// Marks a player who has been invited to a faction, until they accept, decline or the invitation expires.
    /// </summary>
    public class PlayerFactionInvitation : Component
    {
        private static readonly TimeSpan ValidFor = TimeSpan.FromSeconds(60);

        private readonly ulong _factionId;
        private readonly ulong _rankId;
        private readonly string _inviterName;
        private readonly DateTime _expiresAt = DateTime.UtcNow.Add(ValidFor);

        public PlayerFactionInvitation(ulong factionId, ulong rankId, string inviterName)
        {
            _factionId = factionId;
            _rankId = rankId;
            _inviterName = inviterName;
        }

        public ulong GetFactionId()
        {
            return _factionId;
        }

        public ulong GetRankId()
        {
            return _rankId;
        }

        public string GetInviterName()
        {
            return _inviterName;
        }

        public bool IsExpired()
        {
            return DateTime.UtcNow >= _expiresAt;
        }
    }
}
