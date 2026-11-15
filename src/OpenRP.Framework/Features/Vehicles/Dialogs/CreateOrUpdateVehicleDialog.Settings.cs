using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using SampSharp.Entities.SAMP;
using System;

namespace OpenRP.Framework.Features.Vehicles.Dialogs
{
    /// <summary>
    /// The "Server vehicle" and "Virtual world and interior" rows.
    /// </summary>
    public partial class CreateOrUpdateVehicleDialog
    {
        private void ToggleServerVehicle()
        {
            ToggleProperty("Server vehicle", () => Data.ServerVehicle, value => Data.ServerVehicle = value);
        }

        /// <summary>
        /// Either the admin's own virtual world and interior, or two numbers typed in.
        /// </summary>
        private void AskForWorldAndInterior()
        {
            BetterListDialog choiceDialog = new BetterListDialog("Choose", "Back");
            choiceDialog.SetTitle(TitleType.Children, "Vehicle Editor", "Virtual world and interior");
            int useMyWorldRow = choiceDialog.AddRow($"Use my world and interior ({_player.VirtualWorld} and {_player.Interior})");
            int enterWorldRow = choiceDialog.AddRow($"Enter a world and interior ({Data.VirtualWorld} and {Data.Interior})");

            _dialogService.ShowIgnoringDisconnect(_player, choiceDialog, response =>
            {
                if (response.Response != DialogResponse.LeftButton)
                {
                    ShowEditor();
                }
                else if (response.ItemIndex == useMyWorldRow)
                {
                    SetWorldAndInterior(_player.VirtualWorld, _player.Interior);
                    ShowEditor();
                }
                else if (response.ItemIndex == enterWorldRow)
                {
                    AskForWorldAndInteriorText();
                }
                else
                {
                    ShowEditor();
                }
            });
        }

        private void AskForWorldAndInteriorText()
        {
            OpenInputDialog("Edit Virtual World and Interior", $"{Data.VirtualWorld} {Data.Interior}", SetWorldAndInteriorFromInput, ShowEditor,
                "Enter the virtual world and the interior, separated by a space, for example 0 0.");
        }

        private void SetWorldAndInteriorFromInput(string input)
        {
            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2 || !int.TryParse(parts[0], out int virtualWorld) || !int.TryParse(parts[1], out int interior)
                || virtualWorld < 0 || interior < 0)
            {
                OpenMessageDialog("Invalid World and Interior", "Enter two whole numbers, 0 or more: the virtual world, then the interior.", AskForWorldAndInteriorText, ShowEditor);
                return;
            }

            SetWorldAndInterior(virtualWorld, interior);
            ShowEditor();
        }

        private void SetWorldAndInterior(int virtualWorld, int interior)
        {
            Data.VirtualWorld = virtualWorld;
            Data.Interior = interior;
        }
    }
}
