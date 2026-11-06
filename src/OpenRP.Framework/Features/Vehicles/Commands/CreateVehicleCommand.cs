using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Characters.Components;
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
using OpenRP.Framework.Features.Vehicles.Dialogs;
using OpenRP.Framework.Database;

namespace OpenRP.Framework.Features.Vehicles.Commands
{
    public class CreateVehicleCommand : ISystem
    {
        private readonly IDialogService _dialogService;
        private readonly IEntityManager _entityManager;
        private readonly IVehicleManager _vehicleManager;

        public CreateVehicleCommand(IDialogService dialogService, IEntityManager entityManager, IVehicleManager vehicleManager)
        {
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
            if (player.IsPlayerPlayingAsCharacter())
            {
                // Check if the player has the Character
                var characterComponent = player.GetComponent<Character>();
                if (characterComponent == null)
                {
                    player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You need to be playing as a character to use this command.");
                    return;
                }

                // Initialize a new CreateOrUpdateVehicleDataComponent
                var createOrUpdateVehicleDataComponent = new CreateOrUpdateVehicleDataComponent
                {
                    CreateOrUpdateVehicleData = new VehicleModel()
                    {
                        X = player.Position.X,
                        Y = player.Position.Y,
                        Z = player.Position.Z,
                        Rotation = player.Rotation.Z
                    }
                };

                // Open the vehicle creation dialog
                CreateOrUpdateVehicleDialog.Open(player, _dialogService, _entityManager, _vehicleManager, dataContext, createOrUpdateVehicleDataComponent);
            }
        }

    }
}
