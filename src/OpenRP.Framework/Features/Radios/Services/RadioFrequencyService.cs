using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Factions.Services;
using OpenRP.Framework.Features.Radios.Components;
using OpenRP.Framework.Features.Radios.Helpers;
using SampSharp.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Radios.Services
{
    public class RadioFrequencyService : IRadioFrequencyService
    {
        private readonly IEntityManager _entityManager;
        private readonly IFactionService _factionService;
        private readonly IFactionMembershipService _factionMembershipService;

        public RadioFrequencyService(IEntityManager entityManager, IFactionService factionService, IFactionMembershipService factionMembershipService)
        {
            _entityManager = entityManager;
            _factionService = factionService;
            _factionMembershipService = factionMembershipService;
        }

        public RadioFrequency? GetFrequency(uint number)
        {
            return _entityManager.GetComponents<RadioFrequency>().FirstOrDefault(frequency => !frequency.IsDeleted() && frequency.GetNumber() == number);
        }

        public List<RadioFrequency> GetFrequencies()
        {
            return _entityManager.GetComponents<RadioFrequency>()
                .Where(frequency => !frequency.IsDeleted())
                .OrderBy(frequency => frequency.GetNumber())
                .ToList();
        }

        public List<RadioFrequencyFaction> GetFrequencyFactions(RadioFrequency frequency)
        {
            return _entityManager.GetComponents<RadioFrequencyFaction>()
                .Where(frequencyFaction => !frequencyFaction.IsDeleted() && frequencyFaction.GetRadioFrequencyId() == frequency.GetId())
                .ToList();
        }

        public bool CanListen(uint number, Character? character)
        {
            RadioFrequency? frequency = GetFrequency(number);
            if (frequency == null)
            {
                return RadioFrequencyNumbers.IsInOpenRange(number);
            }
            return !frequency.IsEncrypted() || (character != null && IsMember(frequency, character));
        }

        public string? GetTalkError(uint number, Character character)
        {
            RadioFrequency? frequency = GetFrequency(number);
            if (frequency == null)
            {
                return RadioFrequencyNumbers.IsInOpenRange(number) ? null : $"There's no signal on {number}.";
            }
            return IsMember(frequency, character) ? null : $"Only its own people can talk on {GetLabel(number)}.";
        }

        public string GetLabel(uint number)
        {
            RadioFrequency? frequency = GetFrequency(number);
            return frequency == null ? number.ToString() : $"{number} ({frequency.GetName()})";
        }

        public Faction? GetFaction(RadioFrequencyFaction frequencyFaction)
        {
            return _factionService.GetFaction(frequencyFaction.GetFactionId());
        }

        /// <summary>
        /// Whether the character is in one of the frequency's factions, or in a faction one of them controls.
        /// </summary>
        private bool IsMember(RadioFrequency frequency, Character character)
        {
            List<Faction> allowedFactions = GetFrequencyFactions(frequency).Select(GetFaction).OfType<Faction>().ToList();
            return _factionMembershipService.GetMemberships(character)
                .Select(membership => _factionService.GetFaction(membership.GetFactionId()))
                .OfType<Faction>()
                .Any(faction => allowedFactions.Any(allowedFaction => _factionService.IsControlledBy(faction, allowedFaction)));
        }
    }
}
