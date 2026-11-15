using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Jurisdictions.Services;
using OpenRP.Framework.Features.Jurisdictions.Services.Dialogs;
using OpenRP.Framework.Features.Radios.Services;
using OpenRP.Framework.Features.Vehicles.Components;
using OpenRP.Framework.Features.Vehicles.Services;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using OpenRP.Framework.Shared.Dialogs.Helpers;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Vehicles.Dialogs
{
    /// <summary>
    /// The vehicle editor. It edits a copy of the vehicle's data: nothing reaches the database or the live vehicle until
    /// Save. Cancel, or closing the dialog, throws the copy away.
    /// </summary>
    public partial class CreateOrUpdateVehicleDialog
    {
        private readonly Player _player;
        private readonly IDialogService _dialogService;
        private readonly IEntityManager _entityManager;
        private readonly IVehicleManager _vehicleManager;
        private readonly IJurisdictionService _jurisdictionService;
        private readonly IJurisdictionDialogService _jurisdictionDialogService;
        private readonly IRadioService _radioService;
        private readonly IRadioEditorService _radioEditorService;
        private readonly IVehicleKeyService _vehicleKeyService;
        private readonly BaseDataContext _context;
        private readonly CreateOrUpdateVehicleDataComponent _component;

        private CreateOrUpdateVehicleDialog(Player player, IDialogService dialogService, IEntityManager entityManager, IVehicleManager vehicleManager,
            IJurisdictionService jurisdictionService, IJurisdictionDialogService jurisdictionDialogService, IRadioService radioService,
            IRadioEditorService radioEditorService, IVehicleKeyService vehicleKeyService, BaseDataContext context, CreateOrUpdateVehicleDataComponent component)
        {
            _player = player;
            _dialogService = dialogService;
            _entityManager = entityManager;
            _vehicleManager = vehicleManager;
            _jurisdictionService = jurisdictionService;
            _jurisdictionDialogService = jurisdictionDialogService;
            _radioService = radioService;
            _radioEditorService = radioEditorService;
            _vehicleKeyService = vehicleKeyService;
            _context = context;
            _component = component;
        }

        /// <summary>
        /// The data being edited. Every row changes this copy.
        /// </summary>
        private VehicleModel Data => _component.CreateOrUpdateVehicleData;

        public static void Open(Player player, IDialogService dialogService, IEntityManager entityManager, IVehicleManager vehicleManager, IJurisdictionService jurisdictionService, IJurisdictionDialogService jurisdictionDialogService, IRadioService radioService, IRadioEditorService radioEditorService, IVehicleKeyService vehicleKeyService, BaseDataContext context, CreateOrUpdateVehicleDataComponent createOrUpdateVehicleDataComponent)
        {
            CreateOrUpdateVehicleDialog editor = new CreateOrUpdateVehicleDialog(player, dialogService, entityManager, vehicleManager, jurisdictionService, jurisdictionDialogService, radioService, radioEditorService, vehicleKeyService, context, createOrUpdateVehicleDataComponent);
            editor.ShowEditor();
        }

        /// <summary>
        /// Shows the rows. It's shown again after every change, so the rows always show the copy.
        /// </summary>
        private void ShowEditor()
        {
            string dialogName = DialogHelper.GetTitle(_component.EditedVehicle == null
                ? "Vehicle Creator"
                : $"Vehicle Editor{ChatColor.CornflowerBlue} -> {ChatColor.White}{_vehicleManager.DescribeModel(Data)}");

            List<string> dialogColumnHeaders = new List<string>
            {
                ChatColor.CornflowerBlue + "Key",
                ChatColor.CornflowerBlue + "Value"
            };

            TablistDialog dialog = new TablistDialog(dialogName, "Edit", "Cancel", dialogColumnHeaders.ToArray());
            foreach (List<string> row in GetEditorRows())
            {
                dialog.Add(row.ToArray());
            }

            _dialogService.ShowIgnoringDisconnect(_player, dialog, HandleRowChosen);
        }

        /// <summary>
        /// The rows in the order of their indexes in <see cref="HandleRowChosen"/>.
        /// </summary>
        private List<List<string>> GetEditorRows()
        {
            List<List<string>> rows = new List<List<string>>();

            // Values can be empty, so they're never assumed to be set.
            void AddRow(string key, object? value)
            {
                rows.Add(new List<string>
                {
                    ChatColor.White + key,
                    ChatColor.CornflowerBlue + (value?.ToString() ?? "None")
                });
            }

            AddRow("Model ID", _vehicleManager.DescribeModel(Data));
            AddRow("Position (X, Y, Z) + Heading", $"({Data.X}, {Data.Y}, {Data.Z}) + ({Data.Rotation})");
            AddRow("Color 1", Data.Color1);
            AddRow("Color 2", Data.Color2);
            AddRow("Engine On", Data.IsEngineOn);
            AddRow("Lights On", Data.AreLightsOn);
            AddRow("Alarm On", Data.IsAlarmOn);
            AddRow("Doors Locked", Data.AreDoorsLocked);
            AddRow("Bonnet Open", Data.IsBonnetOpen);
            AddRow("Boot Open", Data.IsBootOpen);
            AddRow("Driver Door Open", Data.IsDriverDoorOpen);
            AddRow("Passenger Door Open", Data.IsPassengerDoorOpen);
            AddRow("Back Left Door Open", Data.IsBackLeftDoorOpen);
            AddRow("Back Right Door Open", Data.IsBackRightDoorOpen);
            AddRow("Driver Window Closed", Data.IsDriverWindowClosed);
            AddRow("Passenger Window Closed", Data.IsPassengerWindowClosed);
            AddRow("Back Left Window Closed", Data.IsBackLeftWindowClosed);
            AddRow("Back Right Window Closed", Data.IsBackRightWindowClosed);
            AddRow("Current Health", Data.CurrentHealth);
            AddRow("Max Health", Data.MaxHealth);
            AddRow("Registered in", GetRegisteredInLabel());
            AddRow("Radio", GetRadioLabel());
            AddRow("Owner", GetOwnerLabel());
            AddRow("Server vehicle", Data.ServerVehicle ? "Yes" : "No");
            AddRow("Virtual world and interior", $"{Data.VirtualWorld} and {Data.Interior}");
            AddRow("Give key", GetGiveKeyLabel());
            AddRow("Save", string.Empty);
            return rows;
        }

        private void HandleRowChosen(TablistDialogResponse response)
        {
            if (response.Response != DialogResponse.LeftButton)
            {
                return;
            }

            switch (response.ItemIndex)
            {
                case 0: // Model ID
                    AskForModelId();
                    break;

                case 1: // Position + heading
                    AskForPosition();
                    break;

                case 2: // Color 1
                    AskForInteger("Edit Color 1", Data.Color1, color => Data.Color1 = color, "You have entered an invalid color.");
                    break;

                case 3: // Color 2
                    AskForInteger("Edit Color 2", Data.Color2, color => Data.Color2 = color, "You have entered an invalid color.");
                    break;

                case 4:
                    ToggleProperty("Engine On", () => Data.IsEngineOn, value => Data.IsEngineOn = value);
                    break;

                case 5:
                    ToggleProperty("Lights On", () => Data.AreLightsOn, value => Data.AreLightsOn = value);
                    break;

                case 6:
                    ToggleProperty("Alarm On", () => Data.IsAlarmOn, value => Data.IsAlarmOn = value);
                    break;

                case 7:
                    ToggleProperty("Doors Locked", () => Data.AreDoorsLocked, value => Data.AreDoorsLocked = value);
                    break;

                case 8:
                    ToggleProperty("Bonnet Open", () => Data.IsBonnetOpen, value => Data.IsBonnetOpen = value);
                    break;

                case 9:
                    ToggleProperty("Boot Open", () => Data.IsBootOpen, value => Data.IsBootOpen = value);
                    break;

                case 10:
                    ToggleProperty("Driver Door Open", () => Data.IsDriverDoorOpen, value => Data.IsDriverDoorOpen = value);
                    break;

                case 11:
                    ToggleProperty("Passenger Door Open", () => Data.IsPassengerDoorOpen, value => Data.IsPassengerDoorOpen = value);
                    break;

                case 12:
                    ToggleProperty("Back Left Door Open", () => Data.IsBackLeftDoorOpen, value => Data.IsBackLeftDoorOpen = value);
                    break;

                case 13:
                    ToggleProperty("Back Right Door Open", () => Data.IsBackRightDoorOpen, value => Data.IsBackRightDoorOpen = value);
                    break;

                case 14:
                    ToggleProperty("Driver Window Closed", () => Data.IsDriverWindowClosed, value => Data.IsDriverWindowClosed = value);
                    break;

                case 15:
                    ToggleProperty("Passenger Window Closed", () => Data.IsPassengerWindowClosed, value => Data.IsPassengerWindowClosed = value);
                    break;

                case 16:
                    ToggleProperty("Back Left Window Closed", () => Data.IsBackLeftWindowClosed, value => Data.IsBackLeftWindowClosed = value);
                    break;

                case 17:
                    ToggleProperty("Back Right Window Closed", () => Data.IsBackRightWindowClosed, value => Data.IsBackRightWindowClosed = value);
                    break;

                case 18: // Current health
                    AskForFloat("Edit Current Vehicle Health", Data.CurrentHealth, health => Data.CurrentHealth = health, "You have entered an invalid vehicle health.");
                    break;

                case 19: // Max health
                    AskForFloat("Edit Max Vehicle Health", Data.MaxHealth, health => Data.MaxHealth = health, "You have entered an invalid vehicle health.");
                    break;

                case 20: // Registered in
                    OpenRegisteredIn();
                    break;

                case 21: // Radio
                    ToggleRadio();
                    break;

                case 22: // Owner
                    AskForOwner();
                    break;

                case 23: // Server vehicle
                    ToggleServerVehicle();
                    break;

                case 24: // Virtual world and interior
                    AskForWorldAndInterior();
                    break;

                case 25: // Give key
                    GiveKey();
                    break;

                case 26: // Save
                    SaveVehicleData();
                    break;

                default:
                    _player.SendClientMessage(ChatColor.Red + "This field is not yet editable.");
                    ShowEditor();
                    break;
            }
        }

        /// <summary>
        /// Moves the copy to the position and heading of the admin, or of their vehicle when they're in one.
        /// </summary>
        private void AskForPosition()
        {
            Vector3 position;
            float heading;
            if (_player.InAnyVehicle)
            {
                Vehicle vehiclePlayerIsIn = _entityManager.GetComponent<Vehicle>(_player.Vehicle);
                position = vehiclePlayerIsIn.Position;
                heading = vehiclePlayerIsIn.Angle;
            }
            else
            {
                position = _player.Position;
                heading = _player.Angle;
            }

            OpenMessageDialog("Edit Position + Heading", $"Are you sure that you want to change the position of the vehicle to {position.X}, {position.Y}, {position.Z} with heading {heading}?",
                () =>
                {
                    Data.X = position.X;
                    Data.Y = position.Y;
                    Data.Z = position.Z;
                    Data.Rotation = heading;
                    ShowEditor();
                },
                ShowEditor);
        }

        private void AskForInteger(string title, int currentValue, Action<int> setValue, string invalidText)
        {
            OpenInputDialog(title, currentValue.ToString(), value =>
            {
                if (!int.TryParse(value, out int result))
                {
                    OpenMessageDialog("Invalid Value", invalidText, ShowEditor, ShowEditor);
                    return;
                }

                setValue(result);
                ShowEditor();
            }, ShowEditor);
        }

        private void AskForFloat(string title, float currentValue, Action<float> setValue, string invalidText)
        {
            OpenInputDialog(title, currentValue.ToString(), value =>
            {
                if (!float.TryParse(value, out float result))
                {
                    OpenMessageDialog("Invalid Value", invalidText, ShowEditor, ShowEditor);
                    return;
                }

                setValue(result);
                ShowEditor();
            }, ShowEditor);
        }

        /// <summary>
        /// Flips a yes or no property of the copy. It reaches the vehicle when the editor is saved.
        /// </summary>
        private void ToggleProperty(string propertyName, Func<bool> getProperty, Action<bool> setProperty)
        {
            bool newValue = !getProperty();
            setProperty(newValue);
            _player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"{propertyName} is now {(newValue ? "Yes" : "No")}. It takes effect when you save.");
            ShowEditor();
        }

        private void OpenInputDialog(string title, string defaultValue, Action<string> onInput, Action onReturn, string prompt = "Enter new value:")
        {
            InputDialog inputDialog = new InputDialog()
            {
                Caption = ChatColor.CornflowerBlue + title,
                Content = ChatColor.White + prompt,
                Button1 = "Edit",
                Button2 = "Cancel"
            };

            _dialogService.ShowIgnoringDisconnect(_player, inputDialog, response =>
            {
                if (response.Response != DialogResponse.LeftButton)
                {
                    onReturn();
                    return;
                }
                onInput(response.InputText ?? defaultValue);
            });
        }

        private void OpenMessageDialog(string title, string content, Action onInput, Action onReturn)
        {
            MessageDialog messageDialog = new MessageDialog(ChatColor.CornflowerBlue + title, ChatColor.White + content, "Edit", "Cancel");

            _dialogService.ShowIgnoringDisconnect(_player, messageDialog, response =>
            {
                if (response.Response == DialogResponse.LeftButton)
                {
                    onInput();
                }
                else
                {
                    onReturn();
                }
            });
        }
    }
}
