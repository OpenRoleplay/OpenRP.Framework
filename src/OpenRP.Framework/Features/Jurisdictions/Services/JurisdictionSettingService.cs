using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Factions.Services;
using OpenRP.Framework.Features.Jurisdictions.Components;
using OpenRP.Framework.Features.Jurisdictions.Entities;
using OpenRP.Framework.Features.Jurisdictions.Enums;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Jurisdictions.Services
{
    public class JurisdictionSettingService : IJurisdictionSettingService
    {
        private const string SaveFailedMessage = "The setting couldn't be saved. Another server may have saved it already; try again in a minute.";

        private readonly IJurisdictionService _jurisdictionService;
        private readonly IFactionMembershipService _factionMembershipService;
        private readonly IJurisdictionSettingManager _jurisdictionSettingManager;
        private readonly IEntityManager _entityManager;
        private readonly ILogger<JurisdictionSettingService> _logger;
        private readonly IReadOnlyList<IJurisdictionSetting> _settings;

        public JurisdictionSettingService(IJurisdictionService jurisdictionService, IFactionMembershipService factionMembershipService,
            IJurisdictionSettingManager jurisdictionSettingManager, IEntityManager entityManager, ILogger<JurisdictionSettingService> logger,
            IEnumerable<IJurisdictionSetting> settings)
        {
            _jurisdictionService = jurisdictionService;
            _factionMembershipService = factionMembershipService;
            _jurisdictionSettingManager = jurisdictionSettingManager;
            _entityManager = entityManager;
            _logger = logger;
            _settings = settings.ToList();
        }

        public T GetValue<T>(Jurisdiction jurisdiction, JurisdictionSettingDefinition<T> setting)
        {
            return setting.Deserialize(GetEffectiveStoredValue(jurisdiction, setting));
        }

        public JurisdictionSettingState GetState(Jurisdiction jurisdiction, IJurisdictionSetting setting)
        {
            string effectiveStoredValue = GetEffectiveStoredValue(jurisdiction, setting);
            (JurisdictionSettingSource source, Jurisdiction? sourceJurisdiction) = GetSource(jurisdiction, setting);
            return new JurisdictionSettingState(
                setting.Format(effectiveStoredValue, jurisdiction),
                source,
                sourceJurisdiction,
                GetOwnStoredValue(jurisdiction, setting) != null,
                jurisdiction.IsCountry() && GetOwnBindsSubJurisdictions(jurisdiction, setting));
        }

        public IReadOnlyList<IJurisdictionSetting> GetSettings()
        {
            return _settings;
        }

        public bool CanGovern(Character character, Jurisdiction jurisdiction)
        {
            JurisdictionGovernment? government = _jurisdictionService.GetGovernment(jurisdiction);
            return government != null && _factionMembershipService.IsLeader(character, government.Faction);
        }

        public string? SetValue(Jurisdiction jurisdiction, IJurisdictionSetting setting, string input)
        {
            string? parseError = setting.TryParse(input, jurisdiction, out string storedValue);
            if (parseError != null)
            {
                return parseError;
            }

            string? boundError = GetParentBoundError(jurisdiction, setting, storedValue);
            if (boundError != null)
            {
                return boundError;
            }
            return SaveOwnValue(jurisdiction, setting, storedValue);
        }

        public string? ClearValue(Jurisdiction jurisdiction, IJurisdictionSetting setting)
        {
            JurisdictionSetting? row = FindLiveRow(jurisdiction, setting);
            if (row == null || !row.HasStoredValue())
            {
                return $"{setting.Name} has no value of its own in {jurisdiction.GetName()}.";
            }

            row.SetStoredValue(null);
            SetRowDeletedWhenEmpty(row);
            return null;
        }

        public string? SetBindsSubJurisdictions(Jurisdiction country, IJurisdictionSetting setting, bool binds)
        {
            if (!country.IsCountry())
            {
                return "Only a country can bind its sub-jurisdictions.";
            }

            return SaveBinding(country, setting, binds);
        }

        public bool IsBound(Jurisdiction jurisdiction, IJurisdictionSetting setting)
        {
            Jurisdiction country = _jurisdictionService.GetCountry(jurisdiction);
            return country.GetId() != jurisdiction.GetId() && GetOwnBindsSubJurisdictions(country, setting);
        }

        /// <summary>
        /// The serialized value in effect. Walks down from the country, so each place applies its own value on top of
        /// its parent's, and a loop in the parents stops at the first repeat.
        /// </summary>
        private string GetEffectiveStoredValue(Jurisdiction jurisdiction, IJurisdictionSetting setting)
        {
            string effectiveStoredValue = setting.DefaultValue;
            foreach (Jurisdiction place in GetLineageFromCountry(jurisdiction))
            {
                effectiveStoredValue = GetStoredValueAfterPlace(place, setting, effectiveStoredValue);
            }
            return effectiveStoredValue;
        }

        /// <summary>
        /// The value a place has, given what its parent has: its own value (bound to the parent's when bound), or the
        /// parent's value when it has none of its own.
        /// </summary>
        private string GetStoredValueAfterPlace(Jurisdiction place, IJurisdictionSetting setting, string parentStoredValue)
        {
            string? ownStoredValue = GetOwnStoredValue(place, setting);
            if (ownStoredValue == null)
            {
                return parentStoredValue;
            }
            return IsBound(place, setting) ? setting.Bind(ownStoredValue, parentStoredValue) : ownStoredValue;
        }

        /// <summary>
        /// The jurisdiction whose own value the effective value comes from. Walks down like GetEffectiveStoredValue, so a
        /// value limited by a parent comes from that parent. Null when only the default applies.
        /// </summary>
        private Jurisdiction? GetValueSupplier(Jurisdiction jurisdiction, IJurisdictionSetting setting)
        {
            Jurisdiction? valueSupplier = null;
            foreach (Jurisdiction place in GetLineageFromCountry(jurisdiction))
            {
                valueSupplier = GetSupplierAfterPlace(place, setting, valueSupplier);
            }
            return valueSupplier;
        }

        /// <summary>
        /// The place when its own value is in effect there, otherwise the supplier of its parent's value.
        /// </summary>
        private Jurisdiction? GetSupplierAfterPlace(Jurisdiction place, IJurisdictionSetting setting, Jurisdiction? parentValueSupplier)
        {
            string? ownStoredValue = GetOwnStoredValue(place, setting);
            if (ownStoredValue == null || IsOwnValueLimitedByParent(place, setting, ownStoredValue))
            {
                return parentValueSupplier;
            }
            return place;
        }

        /// <summary>
        /// The country first, then each place down to the jurisdiction itself.
        /// </summary>
        private List<Jurisdiction> GetLineageFromCountry(Jurisdiction jurisdiction)
        {
            List<Jurisdiction> lineage = _jurisdictionService.GetAncestors(jurisdiction);
            lineage.Reverse();
            lineage.Add(jurisdiction);
            return lineage;
        }

        private (JurisdictionSettingSource Source, Jurisdiction? SourceJurisdiction) GetSource(Jurisdiction jurisdiction, IJurisdictionSetting setting)
        {
            string? ownStoredValue = GetOwnStoredValue(jurisdiction, setting);
            if (ownStoredValue != null)
            {
                return GetOwnValueSource(jurisdiction, setting, ownStoredValue);
            }

            Jurisdiction? valueSupplier = GetValueSupplier(jurisdiction, setting);
            return valueSupplier == null
                ? (JurisdictionSettingSource.Default, null)
                : (JurisdictionSettingSource.Inherited, valueSupplier);
        }

        private (JurisdictionSettingSource Source, Jurisdiction? SourceJurisdiction) GetOwnValueSource(Jurisdiction jurisdiction, IJurisdictionSetting setting, string ownStoredValue)
        {
            Jurisdiction? parent = _jurisdictionService.GetParentJurisdiction(jurisdiction);
            if (parent != null && IsOwnValueLimitedByParent(jurisdiction, setting, ownStoredValue))
            {
                return (JurisdictionSettingSource.LimitedByParent, GetLimitingJurisdiction(parent, setting));
            }
            return (JurisdictionSettingSource.Own, null);
        }

        /// <summary>
        /// The jurisdiction that limits a value of its own: the ancestor the parent's value comes from, or the parent
        /// itself when the parent has its own value in effect.
        /// </summary>
        private Jurisdiction GetLimitingJurisdiction(Jurisdiction parent, IJurisdictionSetting setting)
        {
            return GetValueSupplier(parent, setting) ?? parent;
        }

        private bool IsOwnValueLimitedByParent(Jurisdiction jurisdiction, IJurisdictionSetting setting, string ownStoredValue)
        {
            Jurisdiction? parent = _jurisdictionService.GetParentJurisdiction(jurisdiction);
            return parent != null
                && IsBound(jurisdiction, setting)
                && !setting.IsWithinBound(ownStoredValue, GetEffectiveStoredValue(parent, setting));
        }

        /// <summary>
        /// The reason a bound jurisdiction can't take this value, or null when the value fits under its parent's.
        /// </summary>
        private string? GetParentBoundError(Jurisdiction jurisdiction, IJurisdictionSetting setting, string storedValue)
        {
            Jurisdiction? parent = _jurisdictionService.GetParentJurisdiction(jurisdiction);
            if (parent == null || !IsBound(jurisdiction, setting))
            {
                return null;
            }

            string parentStoredValue = GetEffectiveStoredValue(parent, setting);
            if (setting.IsWithinBound(storedValue, parentStoredValue))
            {
                return null;
            }
            return setting.GetBoundError(parentStoredValue, GetLimitingJurisdiction(parent, setting), jurisdiction);
        }

        private string? SaveOwnValue(Jurisdiction jurisdiction, IJurisdictionSetting setting, string storedValue)
        {
            JurisdictionSetting? row = FindRow(jurisdiction, setting);
            if (row != null)
            {
                row.SetStoredValue(storedValue);
                SetRowDeletedWhenEmpty(row);
                return null;
            }

            return TryCreateRow(new JurisdictionSettingModel
            {
                JurisdictionId = jurisdiction.GetId(),
                Key = setting.Key,
                Value = storedValue
            });
        }

        private string? SaveBinding(Jurisdiction country, IJurisdictionSetting setting, bool binds)
        {
            JurisdictionSetting? row = FindRow(country, setting);
            if (row == null)
            {
                // Without a row, not binding is already the case, so there's nothing to store.
                return binds ? TryCreateRow(new JurisdictionSettingModel { JurisdictionId = country.GetId(), Key = setting.Key, BindsSubJurisdictions = true }) : null;
            }

            row.SetBindsSubJurisdictions(binds);
            SetRowDeletedWhenEmpty(row);
            return null;
        }

        /// <summary>
        /// A row that says nothing (no value and no binding) is removed, and a row that says something is kept.
        /// </summary>
        private static void SetRowDeletedWhenEmpty(JurisdictionSetting row)
        {
            row.ProcessDeletion(!row.HasStoredValue() && !row.GetBindsSubJurisdictions());
        }

        /// <summary>
        /// Inserts the row right away. The database's unique index refuses a second row for the same setting.
        /// </summary>
        private string? TryCreateRow(JurisdictionSettingModel model)
        {
            try
            {
                _jurisdictionSettingManager.CreateAndLoad(model);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save setting {Key} of jurisdiction {JurisdictionId}.", model.Key, model.JurisdictionId);
                return SaveFailedMessage;
            }
        }

        private string? GetOwnStoredValue(Jurisdiction jurisdiction, IJurisdictionSetting setting)
        {
            return FindLiveRow(jurisdiction, setting)?.GetStoredValue();
        }

        private bool GetOwnBindsSubJurisdictions(Jurisdiction jurisdiction, IJurisdictionSetting setting)
        {
            return FindLiveRow(jurisdiction, setting)?.GetBindsSubJurisdictions() ?? false;
        }

        private JurisdictionSetting? FindLiveRow(Jurisdiction jurisdiction, IJurisdictionSetting setting)
        {
            JurisdictionSetting? row = FindRow(jurisdiction, setting);
            return row == null || row.IsDeleted() ? null : row;
        }

        /// <summary>
        /// The row of this setting, including one marked for deletion. Writes reuse that row, because the database
        /// still has it and a second row would break the unique index.
        /// </summary>
        private JurisdictionSetting? FindRow(Jurisdiction jurisdiction, IJurisdictionSetting setting)
        {
            return _entityManager.GetComponents<JurisdictionSetting>()
                .FirstOrDefault(row => row.GetJurisdictionId() == jurisdiction.GetId() && row.GetKey() == setting.Key);
        }
    }
}
