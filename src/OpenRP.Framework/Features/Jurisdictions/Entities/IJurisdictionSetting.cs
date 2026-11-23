using OpenRP.Framework.Features.Jurisdictions.Components;

namespace OpenRP.Framework.Features.Jurisdictions.Entities
{
    /// <summary>
    /// A jurisdiction setting as the menus see it: its text, and its values as stored text. Implemented by
    /// JurisdictionSettingDefinition&lt;T&gt;, which is what concrete settings derive from.
    /// </summary>
    public interface IJurisdictionSetting
    {
        /// <summary>
        /// The key the value is stored under, for example banking.maxMonthlyFee. Never rename it once it's stored.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// The name shown in menus.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The text of the input dialog.
        /// </summary>
        string Prompt { get; }

        /// <summary>
        /// Reads what a player typed. Returns the error text, or null when the input is valid, in which case
        /// storedValue holds the value to store.
        /// </summary>
        string? TryParse(string input, Jurisdiction jurisdiction, out string storedValue);

        /// <summary>
        /// The text that shows a stored value in the jurisdiction.
        /// </summary>
        string Format(string storedValue, Jurisdiction jurisdiction);

        /// <summary>
        /// The default value, stored as text.
        /// </summary>
        string DefaultValue { get; }

        /// <summary>
        /// The stored value of a bound jurisdiction, from its own stored value and its parent's.
        /// </summary>
        string Bind(string ownStoredValue, string parentStoredValue);

        /// <summary>
        /// True when the own value already fits under the parent's, so binding leaves it unchanged.
        /// </summary>
        bool IsWithinBound(string ownStoredValue, string parentStoredValue);

        /// <summary>
        /// The reason a bound jurisdiction can't keep its own value, given its parent's stored value, as menu text.
        /// </summary>
        string GetBoundError(string parentStoredValue, Jurisdiction parent, Jurisdiction jurisdiction);
    }
}
