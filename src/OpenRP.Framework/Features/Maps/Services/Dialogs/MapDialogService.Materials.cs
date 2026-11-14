using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Features.Maps.Helpers;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Maps.Services.Dialogs
{
    public partial class MapDialogService
    {
        private const string TexturePrompt = "Enter the texture as model txd texture [ARGB colour], for example 3374 ce_farmxref sw_barnwood1 0x00000000. The colour is optional.";
        private const string MaterialTextPrompt = "Enter text+index+font+size+bold+fontcolor+backcolor+alignment, for example "
            + "Montgomery DMV+5+Arial+36+1+0xFF000000+0x00000000+1. The index is 0 to 15, bold is 0 or 1, the colours are ARGB hex, "
            + "and the alignment is 0 (left), 1 (centre) or 2 (right).";

        private void OpenTextureList(Player player, MapObject mapObject)
        {
            if (!EnsureObjectExists(player, mapObject))
            {
                return;
            }

            BetterListDialog textureDialog = new BetterListDialog("Choose", "Back");
            textureDialog.SetTitle(TitleType.Children, MapsTitle, MapDialogText.GetObjectTitle(mapObject), "Textures");
            Dictionary<int, Action> actions = new Dictionary<int, Action>();
            for (int materialIndex = 0; materialIndex <= MapObjectMaterial.MaxMaterialIndex; materialIndex++)
            {
                int slot = materialIndex;
                actions[textureDialog.AddRow($"{slot}  {MapDialogText.GetMaterialLabel(mapObject, slot)}")] = () => OpenMaterialSlot(player, mapObject, slot);
            }
            ShowActionList(player, textureDialog, actions, () => OpenObjectMenu(player, mapObject));
        }

        private void OpenMaterialSlot(Player player, MapObject mapObject, int materialIndex)
        {
            if (!EnsureObjectExists(player, mapObject))
            {
                return;
            }

            BetterListDialog slotDialog = new BetterListDialog("Choose", "Back");
            slotDialog.SetTitle(TitleType.Children, MapsTitle, MapDialogText.GetObjectTitle(mapObject), $"Material {materialIndex}");
            Dictionary<int, Action> actions = new Dictionary<int, Action>();
            actions[slotDialog.AddRow("Set texture")] = () => OpenTextureInput(player, mapObject, materialIndex);
            actions[slotDialog.AddRow("Clear")] = () => ClearMaterialSlot(player, mapObject, materialIndex);
            ShowActionList(player, slotDialog, actions, () => OpenTextureList(player, mapObject));
        }

        private void OpenTextureInput(Player player, MapObject mapObject, int materialIndex)
        {
            if (!EnsureObjectExists(player, mapObject))
            {
                return;
            }

            OpenTextInput(player, $"Material {materialIndex}", TexturePrompt,
                text => SetTextureFromInput(player, mapObject, materialIndex, text), () => OpenMaterialSlot(player, mapObject, materialIndex));
        }

        private void SetTextureFromInput(Player player, MapObject mapObject, int materialIndex, string text)
        {
            string? error = _mapEditorService.SetTexture(mapObject, materialIndex, text);
            ContinueAfter(player, error, () => OpenTextureList(player, mapObject), () => OpenTextureInput(player, mapObject, materialIndex));
        }

        private void ClearMaterialSlot(Player player, MapObject mapObject, int materialIndex)
        {
            if (!EnsureObjectExists(player, mapObject))
            {
                return;
            }

            string? error = _mapEditorService.ClearMaterial(mapObject, materialIndex);
            ContinueAfter(player, error, () => OpenTextureList(player, mapObject), () => OpenMaterialSlot(player, mapObject, materialIndex));
        }

        private void OpenMaterialTextInput(Player player, MapObject mapObject)
        {
            if (!EnsureObjectExists(player, mapObject))
            {
                return;
            }

            OpenTextInput(player, "Material text", MaterialTextPrompt,
                text => SetMaterialTextFromInput(player, mapObject, text), () => OpenObjectMenu(player, mapObject));
        }

        private void SetMaterialTextFromInput(Player player, MapObject mapObject, string text)
        {
            string? error = _mapEditorService.SetMaterialText(mapObject, text);
            ContinueAfter(player, error, () => OpenObjectMenu(player, mapObject), () => OpenMaterialTextInput(player, mapObject));
        }
    }
}
