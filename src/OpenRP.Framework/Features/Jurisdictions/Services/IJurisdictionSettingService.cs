using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Jurisdictions.Components;
using OpenRP.Framework.Features.Jurisdictions.Entities;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Jurisdictions.Services
{
    /// <summary>
    /// Reading and changing jurisdiction settings: the value each jurisdiction has in effect, where it comes from, and
    /// who may change it. Changes are saved with the next jurisdiction sync. Methods that can fail return the reason, or null.
    /// </summary>
    public interface IJurisdictionSettingService
    {
        /// <summary>
        /// The effective value in the jurisdiction: its own, limited by its parent when bound, otherwise inherited,
        /// otherwise the default.
        /// </summary>
        T GetValue<T>(Jurisdiction jurisdiction, JurisdictionSettingDefinition<T> setting);

        /// <summary>
        /// The effective value as text, and where it comes from.
        /// </summary>
        JurisdictionSettingState GetState(Jurisdiction jurisdiction, IJurisdictionSetting setting);

        /// <summary>
        /// Every registered setting, in registration order.
        /// </summary>
        IReadOnlyList<IJurisdictionSetting> GetSettings();

        /// <summary>
        /// True for the leaders of the faction that governs the jurisdiction.
        /// </summary>
        bool CanGovern(Character character, Jurisdiction jurisdiction);

        /// <summary>
        /// Gives the jurisdiction its own value, parsed from the input. Refused when the input is invalid, or when the
        /// jurisdiction is bound and its parent doesn't allow the value.
        /// </summary>
        string? SetValue(Jurisdiction jurisdiction, IJurisdictionSetting setting, string input);

        /// <summary>
        /// Removes the jurisdiction's own value, so it inherits again. Refused when it has none.
        /// </summary>
        string? ClearValue(Jurisdiction jurisdiction, IJurisdictionSetting setting);

        /// <summary>
        /// Makes a country bind its sub-jurisdictions to its value for this setting, or stops binding them. Only a
        /// country can bind.
        /// </summary>
        string? SetBindsSubJurisdictions(Jurisdiction country, IJurisdictionSetting setting, bool binds);

        /// <summary>
        /// True when the jurisdiction has a parent and its country binds its sub-jurisdictions for this setting.
        /// </summary>
        bool IsBound(Jurisdiction jurisdiction, IJurisdictionSetting setting);
    }
}
