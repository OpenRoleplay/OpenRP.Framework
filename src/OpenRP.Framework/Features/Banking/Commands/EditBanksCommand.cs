using OpenRP.Framework.Features.Banking.Services.Dialogs;
using OpenRP.Framework.Features.Commands.Attributes;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Banking.Commands
{
    public class EditBanksCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Create banks and set their fee, branches, tellers and property accounts, or look up an OBAN.",
            CommandGroups = new string[] { "Admin", "Bank" })]
        public void EditBanks(Player player, IBankDialogService bankDialogService)
        {
            bankDialogService.OpenBankEditor(player);
        }
    }
}
