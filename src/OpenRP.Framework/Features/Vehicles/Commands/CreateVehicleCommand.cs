using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
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
using OpenRP.Framework.Shared.Extensions;
using OpenRP.Framework.Features.Vehicles.Components;
using OpenRP.Framework.Features.Vehicles.Services;
using OpenRP.Framework.Features.Vehicles.Dialogs;
using OpenRP.Framework.Features.Jurisdictions.Services;
using OpenRP.Framework.Features.Jurisdictions.Services.Dialogs;
using OpenRP.Framework.Features.Radios.Services;

namespace OpenRP.Framework.Features.Vehicles.Commands
{
    public class CreateVehicleCommand : ISystem
    {
        // How far in front of the admin the new vehicle appears.
        private const float SpawnDistance = 4.0f;

        private readonly IDialogService _dialogService;
        private readonly IEntityManager _entityManager;
        private readonly IVehicleManager _vehicleManager;
        private readonly IJurisdictionService _jurisdictionService;
        private readonly IJurisdictionDialogService _jurisdictionDialogService;
        private readonly IRadioService _radioService;
        private readonly IRadioEditorService _radioEditorService;
        private readonly IVehicleKeyService _vehicleKeyService;

        public CreateVehicleCommand(IDialogService dialogService, IEntityManager entityManager, IVehicleManager vehicleManager,
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
            Description = "Create a persistent vehicle that can be owned.")]
        public void CreateVehicle(Player player, BaseDataContext dataContext)
        {
            if (!player.IsPlayerPlayingAsCharacter())
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You need to be playing as a character to use this command.");
                return;
            }

            // The new vehicle goes where the admin is looking, in their world and interior, so they can see it appear.
            var spawnPoint = player.GetPointInFront(SpawnDistance);
            var createOrUpdateVehicleDataComponent = new CreateOrUpdateVehicleDataComponent
            {
                CreateOrUpdateVehicleData = new VehicleModel()
                {
                    X = spawnPoint.X,
                    Y = spawnPoint.Y,
                    Z = player.Position.Z,
                    Rotation = player.Angle,
                    VirtualWorld = player.VirtualWorld,
                    Interior = player.Interior
                }
            };

            // Open the vehicle creation dialog
            CreateOrUpdateVehicleDialog.Open(player, _dialogService, _entityManager, _vehicleManager, _jurisdictionService, _jurisdictionDialogService, _radioService, _radioEditorService, _vehicleKeyService, dataContext, createOrUpdateVehicleDataComponent);
        }

    }
}
