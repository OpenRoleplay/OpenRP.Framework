using OpenRP.Framework.Features.Jurisdictions.Components;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Jurisdictions.Entities
{
    /// <summary>
    /// A kind of jurisdiction setting, such as a maximum bank fee. Concrete settings implement the typed members and
    /// are registered with AddJurisdictionSetting&lt;TSetting&gt;(). Values are stored as invariant-culture text.
    /// </summary>
    /// <remarks>
    /// Named JurisdictionSettingDefinition because the component that stores a row is JurisdictionSetting.
    /// </remarks>
    public abstract class JurisdictionSettingDefinition<T> : IJurisdictionSetting
    {
        /// <summary>
        /// The key the value is stored under, for example banking.maxMonthlyFee.
        /// </summary>
        public abstract string Key { get; }

        public abstract string Name { get; }

        public abstract string Prompt { get; }

        public abstract T Default { get; }

        /// <summary>
        /// The default value, stored as text.
        /// </summary>
        public string DefaultValue => Serialize(Default);

        /// <summary>
        /// Reads what a player typed. Returns the error text, or null when the input is valid and value holds it.
        /// </summary>
        protected abstract string? TryParse(string input, Jurisdiction jurisdiction, out T value);

        public abstract string Format(T value, Jurisdiction jurisdiction);

        /// <summary>
        /// How a bound jurisdiction is limited by its parent: the value it may keep, given its own and its parent's.
        /// </summary>
        public abstract T Bind(T own, T parent);

        /// <summary>
        /// The text that says why a bound jurisdiction can't keep its own value, given its parent's value.
        /// </summary>
        public virtual string GetBoundError(T parentValue, Jurisdiction parent, Jurisdiction jurisdiction)
        {
            return $"{parent.GetName()} limits this to {Format(parentValue, jurisdiction)}.";
        }

        public abstract string Serialize(T value);

        public abstract T Deserialize(string storedValue);

        string? IJurisdictionSetting.TryParse(string input, Jurisdiction jurisdiction, out string storedValue)
        {
            string? error = TryParse(input, jurisdiction, out T value);
            storedValue = error == null ? Serialize(value) : string.Empty;
            return error;
        }

        string IJurisdictionSetting.Format(string storedValue, Jurisdiction jurisdiction)
        {
            return Format(Deserialize(storedValue), jurisdiction);
        }

        string IJurisdictionSetting.Bind(string ownStoredValue, string parentStoredValue)
        {
            return BindStoredValues(ownStoredValue, parentStoredValue);
        }

        bool IJurisdictionSetting.IsWithinBound(string ownStoredValue, string parentStoredValue)
        {
            T own = Deserialize(ownStoredValue);
            return EqualityComparer<T>.Default.Equals(own, Bind(own, Deserialize(parentStoredValue)));
        }

        string IJurisdictionSetting.GetBoundError(string parentStoredValue, Jurisdiction parent, Jurisdiction jurisdiction)
        {
            return GetBoundError(Deserialize(parentStoredValue), parent, jurisdiction);
        }

        private string BindStoredValues(string ownStoredValue, string parentStoredValue)
        {
            return Serialize(Bind(Deserialize(ownStoredValue), Deserialize(parentStoredValue)));
        }
    }
}
