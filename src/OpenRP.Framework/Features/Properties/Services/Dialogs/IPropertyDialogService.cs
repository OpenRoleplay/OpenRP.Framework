using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Properties.Services.Dialogs
{
    public interface IPropertyDialogService
    {
        /// <summary>
        /// Opens the admin menu for creating and managing properties and their doors.
        /// </summary>
        void OpenPropertyEditor(Player player);
    }
}
