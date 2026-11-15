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
using OpenRP.Framework.Features.Jurisdictions.Services;
using OpenRP.Framework.Features.Jurisdictions.Services.Dialogs;
using OpenRP.Framework.Features.Radios.Services;
using OpenRP.Framework.Features.Vehicles.Dialogs;

namespace OpenRP.Framework.Features.Vehicles.Commands
{
    public class EditVehicleCommand : ISystem
    {
        private readonly IDialogService _dialogService;
        private readonly IEntityManager _entityManager;
        private readonly IVehicleManager _vehicleManager;
        private readonly IJurisdictionService _jurisdictionService;
        private readonly IJurisdictionDialogService _jurisdictionDialogService;
        private readonly IRadioService _radioService;
        private readonly IRadioEditorService _radioEditorService;
        private readonly IVehicleKeyService _vehicleKeyService;

        public EditVehicleCommand(IDialogService dialogService, IEntityManager entityManager, IVehicleManager vehicleManager,
            IJurisdictionService jurisdictionService, IJurisdictionDialogService jurisdictionDialogService, IRadioService radioService, IRadioEditorService radioEditorService,
            IVehicleKeyService vehicleKeyService)
        {
            _jurisdictionService = jurisdictionService;
            _jurisdictionDialogService = jurisdictionDialogService;
            _radioService = radioService;
            _radioEditorService = radioEditorService;
            _vehicleKeyService = vehicleKeyService;
            _dialogService = dialogService;
            _entityManager = entityManager;
            _vehicleManager = vehicleManager;
        }

        // The database context is a method parameter, not a constructor one: systems are singletons, and a context
        // kept for the whole server run grows and goes stale.
        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Edit a persistent vehicle that can be owned.")]
        public void EditVehicle(Player player, BaseDataContext dataContext)
        {
            if (!player.IsPlayerPlayingAsCharacter())
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You need to be playing as a character to use this command.");
                return;
            }

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

            // The editor gets a copy, so the live vehicle only changes when the editor is saved.
            var liveVehicleData = vehicleDataComponent.VehicleData;
            var createOrUpdateVehicleDataComponent = new CreateOrUpdateVehicleDataComponent
            {
                CreateOrUpdateVehicleData = liveVehicleData.Clone(),
                OriginalVehicleData = liveVehicleData.Clone(),
                EditedVehicle = vehicle
            };

            // Open the vehicle editing dialog
            CreateOrUpdateVehicleDialog.Open(player, _dialogService, _entityManager, _vehicleManager, _jurisdictionService, _jurisdictionDialogService, _radioService, _radioEditorService, _vehicleKeyService, dataContext, createOrUpdateVehicleDataComponent);
        }
    }
}
