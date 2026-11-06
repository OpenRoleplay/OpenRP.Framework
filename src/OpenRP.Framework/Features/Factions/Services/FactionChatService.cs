using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Factions.Services
{
    public class FactionChatService : IFactionChatService
    {
        private static readonly Color FactionChatColor = new Color(140, 170, 255);

        private readonly IEntityManager _entityManager;
        private readonly IFactionService _factionService;
        private readonly IFactionRankService _factionRankService;
        private readonly IFactionMembershipService _factionMembershipService;

        public FactionChatService(IEntityManager entityManager, IFactionService factionService, IFactionRankService factionRankService, IFactionMembershipService factionMembershipService)
        {
            _entityManager = entityManager;
            _factionService = factionService;
            _factionRankService = factionRankService;
            _factionMembershipService = factionMembershipService;
        }

        public void SendFactionOocMessage(Player player, string text)
        {
            if (!player.IsPlayerPlayingAsCharacter())
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You must be playing as a character in order to do this command!");
                return;
            }

            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            FactionMember? membership = _factionMembershipService.GetActiveMembership(character);
            Faction? faction = membership == null ? null : _factionService.GetFaction(membership.GetFactionId());
            if (membership == null || faction == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, GetNoActiveFactionError(character));
                return;
            }

            string rankName = _factionRankService.GetRank(membership.GetRankId())?.GetName() ?? string.Empty;
            string message = $"(( [{faction.GetName()}] {rankName} {character.GetName()}: {text} ))";
            foreach (Player member in GetOnlineMembers(faction))
            {
                member.SendClientMessage(FactionChatColor, message);
            }
        }

        private string GetNoActiveFactionError(Character character)
        {
            return _factionMembershipService.GetMemberships(character).Count == 0
                ? "You are not a member of any faction."
                : "You are in several factions. Choose your active faction in /faction.";
        }

        private List<Player> GetOnlineMembers(Faction faction)
        {
            HashSet<ulong> memberCharacterIds = _factionMembershipService.GetMembers(faction).Select(member => member.GetCharacterId()).ToHashSet();
            return _entityManager.GetComponents<Character>()
                .Where(character => memberCharacterIds.Contains(character.GetId()))
                .Select(character => character.GetPlayer())
                .Where(player => player != null)
                .Select(player => player!)
                .ToList();
        }
    }
}
