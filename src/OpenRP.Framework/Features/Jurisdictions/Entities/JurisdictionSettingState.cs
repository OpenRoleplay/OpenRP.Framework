using OpenRP.Framework.Features.Jurisdictions.Components;
using OpenRP.Framework.Features.Jurisdictions.Enums;

namespace OpenRP.Framework.Features.Jurisdictions.Entities
{
    /// <summary>
    /// A setting as one jurisdiction sees it: its effective value as text, where that comes from, and whether the
    /// jurisdiction has a value of its own. BindsSubJurisdictions is only set on a country.
    /// </summary>
    /// <remarks>
    /// SourceJurisdiction is the jurisdiction whose own value the effective value comes from. For Inherited that's the
    /// jurisdiction whose value is in effect here. It can be a limiting ancestor further up, because a place whose own
    /// value is limited by its parent passes its parent's source on. For LimitedByParent it's the ancestor whose value
    /// sets the limit: the direct parent if that parent has its own value in effect, otherwise the ancestor that
    /// parent's value comes from. For Own and Default it's null.
    /// </remarks>
    public record JurisdictionSettingState(string ValueText, JurisdictionSettingSource Source, Jurisdiction? SourceJurisdiction, bool HasOwnValue, bool BindsSubJurisdictions);
}
