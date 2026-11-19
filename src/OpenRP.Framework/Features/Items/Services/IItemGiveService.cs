using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Items.Services
{
    public interface IItemGiveService
    {
        /// <summary>
        /// Gives the receiver's character new items with their normal defaults, and tells both players. When it's refused, the admin hears why and nothing changes.
        /// </summary>
        void GiveItem(Player admin, Player receiver, int itemId, int amount);
    }
}
