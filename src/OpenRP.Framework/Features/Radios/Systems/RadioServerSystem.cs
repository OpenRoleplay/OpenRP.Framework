using OpenRP.Framework.Features.Radios.Components;
using OpenRP.Framework.Features.Radios.Services;
using OpenRP.Framework.Features.Radios.Services.Dialogs;
using OpenRP.Framework.Shared.ServerEvents.Attributes;
using OpenRP.Framework.Shared.ServerEvents.Entities;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;

namespace OpenRP.Framework.Features.Radios.Systems
{
    public class RadioServerSystem : IServerSystem
    {
        /// <summary>
        /// "Use" on a handheld radio in the inventory opens that radio's menu.
        /// </summary>
        [ServerEvent]
        public void OnPlayerInventoryItemUsed(OnPlayerInventoryItemUsedEventArgs args, IRadioService radioService, IRadioDialogService radioDialogService)
        {
            ulong? radioId = args.InventoryItem.GetRadioId();
            Radio? radio = radioId.HasValue ? radioService.GetRadio(radioId.Value) : null;
            if (radio != null)
            {
                radioDialogService.OpenRadio(args.Player, radio);
            }
        }
    }
}
