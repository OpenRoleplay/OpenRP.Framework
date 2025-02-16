using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Shared.Chat.Enums;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Features.Vehicles.Components;
using OpenRP.Framework.Features.Vehicles.Services;
using OpenRP.Framework.Database;
using OpenRP.Framework.Features.Vehicles.Dialogs;

namespace OpenRP.Framework.Features.Vehicles.Commands
{
    public class EditVehicleCommand : ISystem
    {
        private readonly IDialogService _dialogService;
        private readonly IEntityManager _entityManager;
        private readonly IVehicleManager _vehicleManager;
        private readonly BaseDataContext _context;

        public EditVehicleCommand(IDialogService dialogService, IEntityManager entityManager, IVehicleManager vehicleManager, BaseDataContext context)
        {
            _dialogService = dialogService;
            _entityManager = entityManager;
            _vehicleManager = vehicleManager;
            _context = context;
        }

        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Edit a persistent vehicle that can be owned.")]
        public void EditVehicle(Player player)
        {
            if (player.IsPlayerPlayingAsCharacter())
            {
                // Check if the player is in a vehicle
                if (!player.InAnyVehicle)
                {
                    player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You must be in a vehicle to edit it.");
                    return;
                }

                // Retrieve the vehicle the player is in
                var vehicle = _entityManager.GetComponent<Vehicle>(player.Vehicle);
                if (vehicle == null)
                {
                    player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "Unable to find the vehicle.");
                    return;
                }

                // Get the VehicleDataComponent from the vehicle
                var vehicleDataComponent = vehicle.GetComponent<VehicleDataComponent>();
                if (vehicleDataComponent == null)
                {
                    player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "This vehicle cannot be edited.");
                    return;
                }

                // Initialize the component for editing
                var createOrUpdateVehicleDataComponent = new CreateOrUpdateVehicleDataComponent
                {
                    CreateOrUpdateVehicleData = vehicleDataComponent.VehicleData,
                    EditedVehicle = vehicle
                };

                // Open the vehicle editing dialog
                CreateOrUpdateVehicleDialog.Open(player, _dialogService, _entityManager, _vehicleManager, _context, createOrUpdateVehicleDataComponent);
            }
        }
    }
}
