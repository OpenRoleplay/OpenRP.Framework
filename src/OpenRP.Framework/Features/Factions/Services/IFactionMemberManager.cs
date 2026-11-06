using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Shared.BaseManager.Helpers;

namespace OpenRP.Framework.Features.Factions.Services
{
    public interface IFactionMemberManager : IBaseManager
    {
        /// <summary>
        /// Inserts the membership right away and loads it, so a second join can't slip in before the next sync.
        /// </summary>
        FactionMember CreateAndLoad(FactionMemberModel model);
    }
}
