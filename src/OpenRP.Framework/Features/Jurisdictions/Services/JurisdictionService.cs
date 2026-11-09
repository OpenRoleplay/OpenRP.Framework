using Microsoft.Extensions.Logging;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Factions.Services;
using OpenRP.Framework.Features.Jurisdictions.Components;
using OpenRP.Framework.Features.Jurisdictions.Entities;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Jurisdictions.Services
{
    public class JurisdictionService : IJurisdictionService
    {
        // Plates without a jurisdiction keep the tag every plate had before jurisdictions existed.
        private const string DefaultPlateTag = "SA";

        private readonly IEntityManager _entityManager;
        private readonly IFactionService _factionService;
        private readonly ILogger<JurisdictionService> _logger;

        public JurisdictionService(IEntityManager entityManager, IFactionService factionService, ILogger<JurisdictionService> logger)
        {
            _entityManager = entityManager;
            _factionService = factionService;
            _logger = logger;
        }

        public List<Jurisdiction> GetJurisdictions()
        {
            return _entityManager.GetComponents<Jurisdiction>()
                .Where(jurisdiction => !jurisdiction.IsDeleted())
                .OrderBy(jurisdiction => jurisdiction.GetName(), StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public Jurisdiction? GetJurisdiction(ulong jurisdictionId)
        {
            Jurisdiction? jurisdiction = _entityManager.GetComponent<Jurisdiction>(JurisdictionEntities.GetJurisdictionId((int)jurisdictionId));
            return jurisdiction == null || jurisdiction.IsDeleted() ? null : jurisdiction;
        }

        public Jurisdiction? GetParentJurisdiction(Jurisdiction jurisdiction)
        {
            ulong? parentJurisdictionId = jurisdiction.GetParentJurisdictionId();
            return parentJurisdictionId.HasValue ? GetJurisdiction(parentJurisdictionId.Value) : null;
        }

        public List<Jurisdiction> GetChildJurisdictions(Jurisdiction jurisdiction)
        {
            return GetJurisdictions()
                .Where(child => child.GetParentJurisdictionId() == jurisdiction.GetId())
                .ToList();
        }

        public List<Jurisdiction> GetAncestors(Jurisdiction jurisdiction)
        {
            return GetJurisdictionAndAncestors(jurisdiction).Skip(1).ToList();
        }

        public Jurisdiction GetCountry(Jurisdiction jurisdiction)
        {
            return GetJurisdictionAndAncestors(jurisdiction).Last();
        }

        public List<JurisdictionTreeRow> GetJurisdictionTree()
        {
            List<Jurisdiction> jurisdictions = GetJurisdictions();
            HashSet<ulong> loadedIds = jurisdictions.Select(jurisdiction => jurisdiction.GetId()).ToHashSet();
            List<JurisdictionTreeRow> tree = new List<JurisdictionTreeRow>();
            HashSet<ulong> addedIds = new HashSet<ulong>();

            // A jurisdiction whose parent isn't loaded is shown at the top, so it can still be found and fixed.
            foreach (Jurisdiction top in jurisdictions.Where(jurisdiction => !IsParentLoaded(jurisdiction, loadedIds)))
            {
                AddToTree(tree, addedIds, jurisdictions, top, 0);
            }
            return tree;
        }

        public JurisdictionGovernment? GetGovernment(Jurisdiction jurisdiction)
        {
            foreach (Jurisdiction governedFrom in GetJurisdictionAndAncestors(jurisdiction))
            {
                Faction? faction = GetOwnGovernment(governedFrom);
                if (faction != null)
                {
                    return new JurisdictionGovernment(faction, governedFrom);
                }
            }
            return null;
        }

        public ulong? GetCurrencyId(Jurisdiction jurisdiction)
        {
            return GetJurisdictionAndAncestors(jurisdiction)
                .Select(ancestor => ancestor.GetCurrencyId())
                .FirstOrDefault(currencyId => currencyId.HasValue);
        }

        public List<Jurisdiction> GetJurisdictionsGovernedBy(Faction faction)
        {
            return GetJurisdictions()
                .Where(jurisdiction => GetGovernment(jurisdiction)?.Faction.GetId() == faction.GetId())
                .ToList();
        }

        public List<FactionTaxRegistration> GetTaxRegistrations(Jurisdiction jurisdiction)
        {
            return GetLoadedTaxRegistrations()
                .Where(registration => registration.GetJurisdictionId() == jurisdiction.GetId())
                .ToList();
        }

        public List<Jurisdiction> GetTaxJurisdictions(Faction faction)
        {
            HashSet<ulong> visitedFactionIds = new HashSet<ulong>();
            Faction? current = faction;
            while (current != null && visitedFactionIds.Add(current.GetId()))
            {
                List<Jurisdiction> jurisdictions = GetOwnTaxJurisdictions(current);
                if (jurisdictions.Count > 0)
                {
                    return jurisdictions;
                }
                current = _factionService.GetParentFaction(current);
            }
            return new List<Jurisdiction>();
        }

        public string GetPlateTag(ulong? jurisdictionId)
        {
            Jurisdiction? jurisdiction = jurisdictionId.HasValue ? GetJurisdiction(jurisdictionId.Value) : null;
            string? countryTag = jurisdiction == null ? null : GetCountry(jurisdiction).GetTwoLetterTag();

            // A tag edited by hand in the database could be too short, and plates need exactly two letters.
            return countryTag?.Length == 2 ? countryTag : DefaultPlateTag;
        }

        private IEnumerable<FactionTaxRegistration> GetLoadedTaxRegistrations()
        {
            return _entityManager.GetComponents<FactionTaxRegistration>().Where(registration => !registration.IsDeleted());
        }

        private List<Jurisdiction> GetOwnTaxJurisdictions(Faction faction)
        {
            return GetLoadedTaxRegistrations()
                .Where(registration => registration.GetFactionId() == faction.GetId())
                .Select(registration => GetJurisdiction(registration.GetJurisdictionId()))
                .OfType<Jurisdiction>()
                .OrderBy(jurisdiction => jurisdiction.GetName(), StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        /// <summary>
        /// The jurisdiction's own government, or null when it has none or the faction isn't loaded.
        /// </summary>
        private Faction? GetOwnGovernment(Jurisdiction jurisdiction)
        {
            ulong? factionId = jurisdiction.GetGovernmentFactionId();
            return factionId.HasValue ? _factionService.GetFaction(factionId.Value) : null;
        }

        private static bool IsParentLoaded(Jurisdiction jurisdiction, HashSet<ulong> loadedIds)
        {
            ulong? parentJurisdictionId = jurisdiction.GetParentJurisdictionId();
            return parentJurisdictionId.HasValue && loadedIds.Contains(parentJurisdictionId.Value);
        }

        /// <summary>
        /// Adds the jurisdiction and, below it, the places in it. Skips anything already added, so a loop in the
        /// data can't make the tree endless.
        /// </summary>
        private static void AddToTree(List<JurisdictionTreeRow> tree, HashSet<ulong> addedIds, List<Jurisdiction> jurisdictions, Jurisdiction jurisdiction, int depth)
        {
            if (!addedIds.Add(jurisdiction.GetId()))
            {
                return;
            }

            tree.Add(new JurisdictionTreeRow(jurisdiction, depth));
            foreach (Jurisdiction child in jurisdictions.Where(child => child.GetParentJurisdictionId() == jurisdiction.GetId()))
            {
                AddToTree(tree, addedIds, jurisdictions, child, depth + 1);
            }
        }

        /// <summary>
        /// The jurisdiction followed by every loaded jurisdiction above it, up to its country. Stops at a parent that
        /// isn't loaded, and at a loop in the data, which would otherwise never end.
        /// </summary>
        private List<Jurisdiction> GetJurisdictionAndAncestors(Jurisdiction jurisdiction)
        {
            List<Jurisdiction> jurisdictionAndAncestors = new List<Jurisdiction> { jurisdiction };
            HashSet<ulong> visitedIds = new HashSet<ulong> { jurisdiction.GetId() };

            Jurisdiction? parent = GetParentJurisdiction(jurisdiction);
            while (parent != null)
            {
                if (!visitedIds.Add(parent.GetId()))
                {
                    _logger.LogWarning("Jurisdiction {JurisdictionId} is part of a loop of parent jurisdictions; check ParentJurisdictionId in the database.", parent.GetId());
                    break;
                }
                jurisdictionAndAncestors.Add(parent);
                parent = GetParentJurisdiction(parent);
            }
            return jurisdictionAndAncestors;
        }
    }
}
