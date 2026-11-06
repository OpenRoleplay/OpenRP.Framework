using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Shared.Commands.Services.Dialogs
{
    public interface IHelpDialogService
    {
        /// <summary>
        /// Shows the commands the player may run, grouped as in their CommandGroups, with a details view per command.
        /// </summary>
        void OpenHelp(Player player);
    }
}
