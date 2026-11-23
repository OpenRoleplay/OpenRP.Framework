namespace OpenRP.Framework.Features.Jurisdictions.Enums
{
    /// <summary>
    /// Where the effective value of a jurisdiction setting comes from.
    /// </summary>
    public enum JurisdictionSettingSource
    {
        /// <summary>
        /// The jurisdiction's own value, which its parent doesn't change.
        /// </summary>
        Own,

        /// <summary>
        /// The jurisdiction's own value, limited by its parent because the country binds its sub-jurisdictions.
        /// </summary>
        LimitedByParent,

        /// <summary>
        /// Taken from the nearest ancestor that has a value of its own.
        /// </summary>
        Inherited,

        /// <summary>
        /// Nothing is set in the jurisdiction or any ancestor, so the default applies.
        /// </summary>
        Default
    }
}
