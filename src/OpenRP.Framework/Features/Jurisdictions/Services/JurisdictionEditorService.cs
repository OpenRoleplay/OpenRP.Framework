using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Jurisdictions.Components;
using OpenRP.Framework.Features.Jurisdictions.Entities;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using OpenRP.Framework.Shared.ServerEvents.Services;
using System;
using System.Linq;

namespace OpenRP.Framework.Features.Jurisdictions.Services
{
    public class JurisdictionEditorService : IJurisdictionEditorService
    {
        private const int NameMaxLength = 64;
        private const string TagsFormat = "Enter the 2-, 3- and 4-letter tags separated by spaces, for example SA SAN USSA.";

        private readonly IJurisdictionService _jurisdictionService;
        private readonly IJurisdictionManager _jurisdictionManager;
        private readonly IFactionTaxRegistrationManager _factionTaxRegistrationManager;
        private readonly IServerEventAggregator _serverEventAggregator;
        private readonly ILogger<JurisdictionEditorService> _logger;
        private readonly List<IJurisdictionDeletionGuard> _deletionGuards;

        public JurisdictionEditorService(IJurisdictionService jurisdictionService, IJurisdictionManager jurisdictionManager,
            IFactionTaxRegistrationManager factionTaxRegistrationManager, IServerEventAggregator serverEventAggregator, ILogger<JurisdictionEditorService> logger,
            IEnumerable<IJurisdictionDeletionGuard> deletionGuards)
        {
            _deletionGuards = deletionGuards.ToList();
            _jurisdictionService = jurisdictionService;
            _jurisdictionManager = jurisdictionManager;
            _factionTaxRegistrationManager = factionTaxRegistrationManager;
            _serverEventAggregator = serverEventAggregator;
            _logger = logger;
        }

        public (Jurisdiction? Jurisdiction, string? Error) CreateJurisdiction(string name, string tags, Jurisdiction? parent)
        {
            string? tagsError = GetTagsError(tags, null, out string[] parsedTags);
            string? error = GetNameError(name) ?? tagsError;
            if (error != null)
            {
                return (null, error);
            }

            JurisdictionModel model = new JurisdictionModel
            {
                Name = name.Trim(),
                TwoLetterTag = parsedTags[0],
                ThreeLetterTag = parsedTags[1],
                FourLetterTag = parsedTags[2],
                ParentJurisdictionId = parent?.GetId()
            };
            return TryCreateJurisdiction(model);
        }

        public string? RenameJurisdiction(Jurisdiction jurisdiction, string name)
        {
            string? error = GetNameError(name);
            if (error != null)
            {
                return error;
            }

            jurisdiction.SetName(name.Trim());
            return null;
        }

        public string? SetTags(Jurisdiction jurisdiction, string tags)
        {
            string? error = GetTagsError(tags, jurisdiction, out string[] parsedTags);
            if (error != null)
            {
                return error;
            }

            jurisdiction.SetTags(parsedTags[0], parsedTags[1], parsedTags[2]);
            PublishPlateTagChanged(jurisdiction);
            return null;
        }

        public void SetCurrency(Jurisdiction jurisdiction, ulong? currencyId)
        {
            jurisdiction.SetCurrencyId(currencyId);
        }

        public string? MoveJurisdiction(Jurisdiction jurisdiction, Jurisdiction? parent)
        {
            if (parent != null && IsSelfOrInside(parent, jurisdiction))
            {
                return "A jurisdiction can't lie in itself or in one of the places inside it.";
            }

            jurisdiction.SetParentJurisdictionId(parent?.GetId());
            PublishPlateTagChanged(jurisdiction);
            return null;
        }

        public void SetGovernment(Jurisdiction jurisdiction, Faction? faction)
        {
            ulong? oldFactionId = jurisdiction.GetGovernmentFactionId();
            ulong? newFactionId = faction?.GetId();
            if (oldFactionId == newFactionId)
            {
                return;
            }

            jurisdiction.SetGovernmentFactionId(newFactionId);

            // Not awaited: the dispatcher logs each handler's errors itself.
            _ = _serverEventAggregator.PublishAsync(new OnJurisdictionGovernmentChangedEventArgs
            {
                JurisdictionId = jurisdiction.GetId(),
                OldGovernmentFactionId = oldFactionId,
                NewGovernmentFactionId = newFactionId
            });
        }

        public string? AddTaxRegistration(Jurisdiction jurisdiction, Faction faction)
        {
            if (_jurisdictionService.GetTaxRegistrations(jurisdiction).Any(registration => registration.GetFactionId() == faction.GetId()))
            {
                return $"{faction.GetName()} is registered in {jurisdiction.GetName()} already.";
            }

            try
            {
                _factionTaxRegistrationManager.CreateAndLoad(new FactionTaxRegistrationModel { FactionId = faction.GetId(), JurisdictionId = jurisdiction.GetId() });
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register faction {FactionId} in jurisdiction {JurisdictionId}.", faction.GetId(), jurisdiction.GetId());
                return "The registration couldn't be saved. Another server may have added it already; try again in a minute.";
            }
        }

        public void RemoveTaxRegistration(FactionTaxRegistration registration)
        {
            registration.ProcessDeletion();
        }

        public string? DeleteJurisdiction(Jurisdiction jurisdiction)
        {
            if (_jurisdictionService.GetChildJurisdictions(jurisdiction).Count > 0)
            {
                return "Other jurisdictions lie in this one. Move or delete them first.";
            }

            // Other features, such as banks, may depend on this jurisdiction.
            string? guardError = _deletionGuards.Select(guard => guard.GetDeletionError(jurisdiction)).FirstOrDefault(error => error != null);
            if (guardError != null)
            {
                return guardError;
            }

            // The database removes these too, but loaded registrations would stay in memory until the next sync.
            foreach (FactionTaxRegistration registration in _jurisdictionService.GetTaxRegistrations(jurisdiction))
            {
                registration.ProcessDeletion();
            }
            jurisdiction.ProcessDeletion();

            // Properties and vehicles in it lose their link. Not awaited: the dispatcher logs each handler's errors itself.
            _ = _serverEventAggregator.PublishAsync(new OnJurisdictionDeletedEventArgs { JurisdictionId = jurisdiction.GetId() });
            return null;
        }

        /// <summary>
        /// Inserts the jurisdiction right away. The tags are checked against the loaded jurisdictions first; the
        /// database's unique indexes still catch a tag another server took in the meantime.
        /// </summary>
        private (Jurisdiction? Jurisdiction, string? Error) TryCreateJurisdiction(JurisdictionModel model)
        {
            try
            {
                return (_jurisdictionManager.CreateAndLoad(model), null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create jurisdiction {Name}.", model.Name);
                return (null, "The jurisdiction couldn't be saved. Another server may have taken one of its tags; try again in a minute.");
            }
        }

        /// <summary>
        /// Whether the candidate is the jurisdiction itself or lies somewhere inside it.
        /// </summary>
        private bool IsSelfOrInside(Jurisdiction candidate, Jurisdiction jurisdiction)
        {
            return candidate.GetId() == jurisdiction.GetId()
                || _jurisdictionService.GetAncestors(candidate).Any(ancestor => ancestor.GetId() == jurisdiction.GetId());
        }

        private void PublishPlateTagChanged(Jurisdiction jurisdiction)
        {
            // Not awaited: the dispatcher logs each handler's errors itself.
            _ = _serverEventAggregator.PublishAsync(new OnJurisdictionPlateTagChangedEventArgs { JurisdictionId = jurisdiction.GetId() });
        }

        /// <summary>
        /// Checks the three tags and hands them back upper-cased. The jurisdiction being changed may keep its own tags.
        /// </summary>
        private string? GetTagsError(string tags, Jurisdiction? changedJurisdiction, out string[] parsedTags)
        {
            parsedTags = tags.Trim().ToUpperInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parsedTags.Length != 3)
            {
                return TagsFormat;
            }

            for (int index = 0; index < parsedTags.Length; index++)
            {
                string? error = GetTagError(parsedTags[index], index + 2, changedJurisdiction);
                if (error != null)
                {
                    return error;
                }
            }
            return null;
        }

        private string? GetTagError(string tag, int length, Jurisdiction? changedJurisdiction)
        {
            if (tag.Length != length || !tag.All(IsAsciiLetter))
            {
                return $"{tag} isn't a valid tag. The {length}-letter tag must be exactly {length} letters (A to Z). {TagsFormat}";
            }

            Jurisdiction? owner = _jurisdictionService.GetJurisdictions()
                .FirstOrDefault(jurisdiction => jurisdiction.GetId() != changedJurisdiction?.GetId() && GetTagOfLength(jurisdiction, length) == tag);
            return owner == null ? null : $"The tag {tag} is used by {owner.GetName()} already.";
        }

        private static string GetTagOfLength(Jurisdiction jurisdiction, int length)
        {
            return length switch
            {
                2 => jurisdiction.GetTwoLetterTag(),
                3 => jurisdiction.GetThreeLetterTag(),
                _ => jurisdiction.GetFourLetterTag()
            };
        }

        private static bool IsAsciiLetter(char character)
        {
            return character >= 'A' && character <= 'Z';
        }

        private static string? GetNameError(string name)
        {
            string trimmedName = name.Trim();
            if (trimmedName.Length == 0)
                return "The jurisdiction name can't be empty.";
            if (trimmedName.Length > NameMaxLength)
                return $"The jurisdiction name can be at most {NameMaxLength} characters long.";
            return null;
        }
    }
}
