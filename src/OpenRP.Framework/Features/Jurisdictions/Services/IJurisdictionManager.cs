using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Jurisdictions.Components;
using OpenRP.Framework.Shared.BaseManager.Helpers;

namespace OpenRP.Framework.Features.Jurisdictions.Services
{
    public interface IJurisdictionManager : IBaseManager
    {
        /// <summary>
        /// Inserts the jurisdiction right away and loads it, so sub-jurisdictions can be created under it immediately.
        /// </summary>
        Jurisdiction CreateAndLoad(JurisdictionModel model);
    }
}
