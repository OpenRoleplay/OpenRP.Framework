using OpenRP.Framework.Features.Vehicles.Components;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Dialogs.Helpers;
using OpenRP.Framework.Shared;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Features.Vehicles.Services;
using OpenRP.Framework.Database;

namespace OpenRP.Framework.Features.Vehicles.Dialogs
{
    public class CreateOrUpdateVehicleDialog
    {
        public static void Open(Player player, IDialogService dialogService, IEntityManager entityManager, IVehicleManager vehicleManager, BaseDataContext context, CreateOrUpdateVehicleDataComponent createOrUpdateVehicleDataComponent)
        {
            if (createOrUpdateVehicleDataComponent != null)
            {
                // Set the dialog title based on whether we're editing or creating
                StringBuilder dialogNameBuilder = new StringBuilder();
                if (createOrUpdateVehicleDataComponent.EditedVehicle == null)
                {
                    dialogNameBuilder.Append("Vehicle Creator");
                }
                else
                {
                    dialogNameBuilder.Append("Vehicle Editor");
                    dialogNameBuilder.Append(ChatColor.CornflowerBlue + " -> " + ChatColor.White);
                    dialogNameBuilder.Append(createOrUpdateVehicleDataComponent.EditedVehicle.Model.ToString());
                }

                string dialogName = DialogHelper.GetTitle(dialogNameBuilder.ToString());

                // Fill dialog headers
                List<string> dialogColumnHeaders = new List<string>
                {
                    ChatColor.CornflowerBlue + "Key",
                    ChatColor.CornflowerBlue + "Value"
                };

                // Prepare rows for each vehicle property
                List<List<string>> dialogRows = new List<List<string>>();

                void AddRow(string key, object value)
                {
                    dialogRows.Add(new List<string>
                    {
                        ChatColor.White + key,
                        ChatColor.CornflowerBlue + value.ToString()
                    });
                }

                AddRow("Model ID", createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.ModelId);
                AddRow("Position (X, Y, Z) + Rotation (Z)", $"({createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.X}, {createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.Y}, {createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.Z}) + ({createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.Rotation})");
                AddRow("Color 1", createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.Color1);
                AddRow("Color 2", createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.Color2);
                AddRow("Engine On", createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsEngineOn);
                AddRow("Lights On", createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.AreLightsOn);
                AddRow("Alarm On", createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsAlarmOn);
                AddRow("Doors Locked", createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.AreDoorsLocked);
                AddRow("Bonnet Open", createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsBonnetOpen);
                AddRow("Boot Open", createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsBootOpen);
                AddRow("Driver Door Open", createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsDriverDoorOpen);
                AddRow("Passenger Door Open", createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsPassengerDoorOpen);
                AddRow("Back Left Door Open", createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsBackLeftDoorOpen);
                AddRow("Back Right Door Open", createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsBackRightDoorOpen);
                AddRow("Driver Window Closed", createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsDriverWindowClosed);
                AddRow("Passenger Window Closed", createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsPassengerWindowClosed);
                AddRow("Back Left Window Closed", createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsBackLeftWindowClosed);
                AddRow("Back Right Window Closed", createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsBackRightWindowClosed);
                AddRow("Current Health", createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.CurrentHealth);
                AddRow("Max Health", createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.MaxHealth);
                AddRow("Save", string.Empty);

                // Create the dialog
                TablistDialog dialog = new TablistDialog(
                    dialogName,
                    "Edit",
                    "Cancel",
                    dialogColumnHeaders.ToArray());

                foreach (List<string> rows in dialogRows)
                {
                    dialog.Add(rows.ToArray());
                }

                // Dialog Response
                void DialogHandler(TablistDialogResponse r)
                {
                    if (r.Response != DialogResponse.LeftButton)
                    {
                        return;
                    }

                    int index = r.ItemIndex;

                    switch (index)
                    {
                        case 0: // Model ID
                            OpenInputDialog(player, dialogService, "Edit Model ID", createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.ModelId.ToString(), value =>
                            {
                                bool invalidId = false;
                                if (int.TryParse(value, out int result))
                                {
                                    if (result >= 400 && result <= 611)
                                    {
                                        //createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.ModelId = result;

                                        Open(player, dialogService, entityManager, vehicleManager, context, createOrUpdateVehicleDataComponent);
                                    }
                                    else
                                    {
                                        invalidId = true;
                                    }
                                }
                                else
                                {
                                    invalidId = true;
                                }

                                if (invalidId)
                                {
                                    OpenMessageDialog(player, dialogService, "Invalid Id", "You have entered an invalid id. Make sure the id is valid and between 400 and 611.",
                                    () => // Left button
                                    {
                                        Open(player, dialogService, entityManager, vehicleManager, context, createOrUpdateVehicleDataComponent);
                                    },
                                    () => // Right button
                                    {
                                        Open(player, dialogService, entityManager, vehicleManager, context, createOrUpdateVehicleDataComponent);
                                    });
                                }
                            });
                            break;

                        case 1: // Position
                            Vector3 newPosition;
                            float newRotation;
                            if (player.InAnyVehicle)
                            {
                                // Retrieve the VehicleComponent
                                Vehicle vehiclePlayerIsIn = entityManager.GetComponent<Vehicle>(player.Vehicle);
                                newPosition = vehiclePlayerIsIn.Position;
                                newRotation = vehiclePlayerIsIn.Rotation.Z;
                            }
                            else
                            {
                                newPosition = player.Position;
                                newRotation = player.Angle;
                            }

                            OpenMessageDialog(player, dialogService, "Edit Position + Rotation", $"Are you sure that you want to change the position of the vehicle to {newPosition.X}, {newPosition.Y}, {newPosition.Z} with rotation {newRotation}?",
                            () => // Left button
                            {
                                createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.X = newPosition.X;
                                createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.Y = newPosition.Y;
                                createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.Z = newPosition.Z;
                                createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.Rotation = newRotation;

                                Open(player, dialogService, entityManager, vehicleManager, context, createOrUpdateVehicleDataComponent);
                            },
                            () => // Right button
                            {
                                Open(player, dialogService, entityManager, vehicleManager, context, createOrUpdateVehicleDataComponent);
                            });
                            break;

                        case 2: // Color 1
                            OpenInputDialog(player, dialogService, "Edit Color 1", createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.Color1.ToString(), value =>
                            {
                                bool invalidColor = false;
                                if (int.TryParse(value, out int result))
                                {
                                    createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.Color1 = result;

                                    if (createOrUpdateVehicleDataComponent.EditedVehicle != null)
                                    {
                                        vehicleManager.UpdateVehicle(createOrUpdateVehicleDataComponent.EditedVehicle, createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData);
                                    }

                                    Open(player, dialogService, entityManager, vehicleManager, context, createOrUpdateVehicleDataComponent);
                                }
                                else
                                {
                                    invalidColor = true;
                                }

                                if (invalidColor)
                                {
                                    OpenMessageDialog(player, dialogService, "Invalid Color", "You have entered an invalid Color.",
                                    () => // Left button
                                    {
                                        Open(player, dialogService, entityManager, vehicleManager, context, createOrUpdateVehicleDataComponent);
                                    },
                                    () => // Right button
                                    {
                                        Open(player, dialogService, entityManager, vehicleManager, context, createOrUpdateVehicleDataComponent);
                                    });
                                }
                            });
                            break;

                        case 3: // Color 2
                            OpenInputDialog(player, dialogService, "Edit Color 2", createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.Color2.ToString(), value =>
                            {
                                bool invalidColor = false;
                                if (int.TryParse(value, out int result))
                                {
                                    createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.Color2 = result;

                                    if (createOrUpdateVehicleDataComponent.EditedVehicle != null)
                                    {
                                        vehicleManager.UpdateVehicle(createOrUpdateVehicleDataComponent.EditedVehicle, createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData);
                                    }

                                    Open(player, dialogService, entityManager, vehicleManager, context, createOrUpdateVehicleDataComponent);
                                }
                                else
                                {
                                    invalidColor = true;
                                }

                                if (invalidColor)
                                {
                                    OpenMessageDialog(player, dialogService, "Invalid Color", "You have entered an invalid Color.",
                                    () => // Left button
                                    {
                                        Open(player, dialogService, entityManager, vehicleManager, context, createOrUpdateVehicleDataComponent);
                                    },
                                    () => // Right button
                                    {
                                        Open(player, dialogService, entityManager, vehicleManager, context, createOrUpdateVehicleDataComponent);
                                    });
                                }
                            });
                            break;

                        case 4: // Engine On/Off

                            createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsEngineOn = !createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsEngineOn;

                            // Show a confirmation message
                            string engineStatus = createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsEngineOn ? "on" : "off";
                            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"Vehicle engine turned {engineStatus}.");

                            // Re-open the main dialog after the change
                            Open(player, dialogService, entityManager, vehicleManager, context, createOrUpdateVehicleDataComponent);
                            break;

                        case 5: // Lights On/Off

                            createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.AreLightsOn = !createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.AreLightsOn;

                            // Show a confirmation message
                            string lightStatus = createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.AreLightsOn ? "on" : "off";
                            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"Vehicle lights turned {lightStatus}.");

                            // Re-open the main dialog after the change
                            Open(player, dialogService, entityManager, vehicleManager, context, createOrUpdateVehicleDataComponent);
                            break;

                        case 6: // Alarm On/Off
                            ToggleProperty(
                                player,
                                createOrUpdateVehicleDataComponent,
                                dialogService,
                                entityManager,
                                vehicleManager,
                                context,
                                "Is Alarm On",
                                () => createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsAlarmOn,
                                value => createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsAlarmOn = value
                            );
                            break;

                        case 7: // Doors Locked/Unlocked
                            ToggleProperty(
                                player,
                                createOrUpdateVehicleDataComponent,
                                dialogService,
                                entityManager,
                                vehicleManager,
                                context,
                                "Are Doors Locked",
                                () => createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.AreDoorsLocked,
                                value => createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.AreDoorsLocked = value
                            );
                            break;

                        case 8: // Bonnet Open/Closed
                            ToggleProperty(
                                player,
                                createOrUpdateVehicleDataComponent,
                                dialogService,
                                entityManager,
                                vehicleManager,
                                context,
                                "Is Bonnet Open",
                                () => createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsBonnetOpen,
                                value => createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsBonnetOpen = value
                            );
                            break;

                        case 9: // Boot Open/Closed
                            ToggleProperty(
                                player,
                                createOrUpdateVehicleDataComponent,
                                dialogService,
                                entityManager,
                                vehicleManager,
                                context,
                                "Is Boot Open",
                                () => createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsBootOpen,
                                value => createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsBootOpen = value
                            );
                            break;

                        case 10: // Driver Door Open/Closed
                            ToggleProperty(
                                player,
                                createOrUpdateVehicleDataComponent,
                                dialogService,
                                entityManager,
                                vehicleManager,
                                context,
                                "Is Driver Door Open",
                                () => createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsDriverDoorOpen,
                                value => createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsDriverDoorOpen = value
                            );
                            break;

                        case 11: // Passenger Door Open/Closed
                            ToggleProperty(
                                player,
                                createOrUpdateVehicleDataComponent,
                                dialogService,
                                entityManager,
                                vehicleManager,
                                context,
                                "Is Passenger Door Open",
                                () => createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsPassengerDoorOpen,
                                value => createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsPassengerDoorOpen = value
                            );
                            break;

                        case 12: // Back Left Door Open/Closed
                            ToggleProperty(
                                player,
                                createOrUpdateVehicleDataComponent,
                                dialogService,
                                entityManager,
                                vehicleManager,
                                context,
                                "Is Back Left Door Open",
                                () => createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsBackLeftDoorOpen,
                                value => createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsBackLeftDoorOpen = value
                            );
                            break;

                        case 13: // Back Right Door Open/Closed
                            ToggleProperty(
                                player,
                                createOrUpdateVehicleDataComponent,
                                dialogService,
                                entityManager,
                                vehicleManager,
                                context,
                                "Is Back Right Door Open",
                                () => createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsBackRightDoorOpen,
                                value => createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsBackRightDoorOpen = value
                            );
                            break;

                        case 14: // Driver Window Closed/Open
                            ToggleProperty(
                                player,
                                createOrUpdateVehicleDataComponent,
                                dialogService,
                                entityManager,
                                vehicleManager,
                                context,
                                "Is Driver Window Closed",
                                () => createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsDriverWindowClosed,
                                value => createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsDriverWindowClosed = value
                            );
                            break;

                        case 15: // Passenger Window Closed/Open
                            ToggleProperty(
                                player,
                                createOrUpdateVehicleDataComponent,
                                dialogService,
                                entityManager,
                                vehicleManager,
                                context,
                                "Is Passenger Window Closed",
                                () => createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsPassengerWindowClosed,
                                value => createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsPassengerWindowClosed = value
                            );
                            break;

                        case 16: // Back Left Window Closed/Open
                            ToggleProperty(
                                player,
                                createOrUpdateVehicleDataComponent,
                                dialogService,
                                entityManager,
                                vehicleManager,
                                context,
                                "Is Back Left Window Closed",
                                () => createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsBackLeftWindowClosed,
                                value => createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsBackLeftWindowClosed = value
                            );
                            break;

                        case 17: // Back Right Window Closed/Open
                            ToggleProperty(
                                player,
                                createOrUpdateVehicleDataComponent,
                                dialogService,
                                entityManager,
                                vehicleManager,
                                context,
                                "Is Back Right Window Closed",
                                () => createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsBackRightWindowClosed,
                                value => createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.IsBackRightWindowClosed = value
                            );
                            break;

                        case 18: // Current Vehicle Health
                            OpenInputDialog(player, dialogService, "Edit Current Vehicle Health", createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.CurrentHealth.ToString(), value =>
                            {
                                bool invalidHealth = false;
                                if (float.TryParse(value, out float result))
                                {
                                    createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.CurrentHealth = result;

                                    Open(player, dialogService, entityManager, vehicleManager, context, createOrUpdateVehicleDataComponent);
                                }
                                else
                                {
                                    invalidHealth = true;
                                }

                                if (invalidHealth)
                                {
                                    OpenMessageDialog(player, dialogService, "Invalid Vehicle Health", "You have entered an invalid Vehicle Health.",
                                    () => // Left button
                                    {
                                        Open(player, dialogService, entityManager, vehicleManager, context, createOrUpdateVehicleDataComponent);
                                    },
                                    () => // Right button
                                    {
                                        Open(player, dialogService, entityManager, vehicleManager, context, createOrUpdateVehicleDataComponent);
                                    });
                                }
                            });
                            break;

                        case 19: // mAX Vehicle Health
                            OpenInputDialog(player, dialogService, "Edit Max Vehicle Health", createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.MaxHealth.ToString(), value =>
                            {
                                bool invalidHealth = false;
                                if (float.TryParse(value, out float result))
                                {
                                    createOrUpdateVehicleDataComponent.CreateOrUpdateVehicleData.MaxHealth = result;

                                    Open(player, dialogService, entityManager, vehicleManager, context, createOrUpdateVehicleDataComponent);
                                }
                                else
                                {
                                    invalidHealth = true;
                                }

                                if (invalidHealth)
                                {
                                    OpenMessageDialog(player, dialogService, "Invalid Vehicle Health", "You have entered an invalid Vehicle Health.",
                                    () => // Left button
                                    {
                                        Open(player, dialogService, entityManager, vehicleManager, context, createOrUpdateVehicleDataComponent);
                                    },
                                    () => // Right button
                                    {
                                        Open(player, dialogService, entityManager, vehicleManager, context, createOrUpdateVehicleDataComponent);
                                    });
                                }
                            });
                            break;

                        case 20: // Save Vehicle Data
                            SaveVehicleData(createOrUpdateVehicleDataComponent, vehicleManager, context);
                            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "Vehicle data has been saved successfully.");
                            break;

                        default:
                            player.SendClientMessage(ChatColor.Red + "This field is not yet editable.");
                            break;
                    }
                };

                // Show the dialog to the player
                dialogService.Show(player, dialog, DialogHandler);
            }
        }

        private static void OpenInputDialog(Player player, IDialogService dialogService, string title, string defaultValue, Action<string> onInput, Action onReturn = null)
        {
            InputDialog inputDialog = new InputDialog()
            {
                Caption = ChatColor.CornflowerBlue + title,
                Content = ChatColor.White + $"Enter new value:",
                Button1 = "Edit",
                Button2 = "Cancel"
            };

            void DialogHandler(InputDialogResponse r)
            {
                string value = r.InputText;

                if (value == null)
                {
                    value = defaultValue;
                }

                if (r.Response == DialogResponse.LeftButton)
                {
                    onInput?.Invoke(value);
                }
                else
                {
                    onReturn?.Invoke();
                }
            };

            dialogService.Show(player, inputDialog, DialogHandler);
        }

        private static void OpenMessageDialog(Player player, IDialogService dialogService, string title, string content, Action onInput, Action onReturn)
        {
            MessageDialog messageDialog = new MessageDialog(ChatColor.CornflowerBlue + title, ChatColor.White + content, "Edit", "Cancel");

            void DialogHandler(MessageDialogResponse r)
            {
                if (r.Response == DialogResponse.LeftButton)
                {
                    onInput?.Invoke();
                }
                else
                {
                    onReturn?.Invoke();
                }
            };

            dialogService.Show(player, messageDialog, DialogHandler);
        }

        private static void ToggleProperty(Player player, CreateOrUpdateVehicleDataComponent component, IDialogService dialogService, IEntityManager entityManager, IVehicleManager vehicleManager, BaseDataContext context, string propertyName, Func<bool> getProperty, Action<bool> setProperty)
        {
            // Get the current value of the property
            bool currentValue = getProperty();

            // Toggle the value
            bool newValue = !currentValue;

            // Set the new value
            setProperty(newValue);

            // Show a confirmation message
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"{propertyName} is now {newValue.ToString()}.");

            if (component.EditedVehicle != null)
            {
                vehicleManager.UpdateVehicle(component.EditedVehicle, component.CreateOrUpdateVehicleData);
            }

            // Re-open the main dialog after the change
            Open(player, dialogService, entityManager, vehicleManager, context, component);
        }

        private static void SaveVehicleData(CreateOrUpdateVehicleDataComponent component, IVehicleManager vehicleManager, BaseDataContext context)
        {
            var vehicleData = component.CreateOrUpdateVehicleData;

            if (vehicleData.Id == 0)
            {
                // Insert new vehicle data
                context.Vehicles.Add(vehicleData);
            }
            else
            {
                // Update existing vehicle data
                context.Vehicles.Update(vehicleData);
                if (component.EditedVehicle != null)
                {
                    vehicleManager.UpdateVehicle(component.EditedVehicle, vehicleData);
                }
            }

            context.SaveChanges();
            vehicleManager.LoadAndUnloadVehicles();
        }
    }
}
