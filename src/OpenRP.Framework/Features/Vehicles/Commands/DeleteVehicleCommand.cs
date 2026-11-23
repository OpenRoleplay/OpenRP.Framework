using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Vehicles.Components;
using OpenRP.Framework.Features.Vehicles.Dialogs;
using OpenRP.Framework.Features.Vehicles.Helpers;
using OpenRP.Framework.Features.Vehicles.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System.Linq;

namespace OpenRP.Framework.Features.Vehicles.Commands
{
    public class DeleteVehicleCommand : ISystem
    {
        private readonly IEntityManager _entityManager;
        private readonly IDialogService _dialogService;
        private readonly IVehicleManager _vehicleManager;
        private readonly IVehicleInfoService _vehicleInfoService;

        public DeleteVehicleCommand(IEntityManager entityManager, IDialogService dialogService, IVehicleManager vehicleManager, IVehicleInfoService vehicleInfoService)
        {
            _entityManager = entityManager;
            _dialogService = dialogService;
            _vehicleManager = vehicleManager;
            _vehicleInfoService = vehicleInfoService;
        }

        // The database context is a method parameter, not a constructor one: systems are singletons, and a context
        // kept for the whole server run grows and goes stale.
        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Delete a persistent vehicle for good.")]
        public void DeleteVehicle(Player player, BaseDataContext dataContext)
        {
            Vehicle? vehicle = player.InAnyVehicle ? player.Vehicle : NearbyVehicleFinder.FindNearestSavedVehicle(_entityManager, _vehicleInfoService, player);
            if (vehicle == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "Sit in the vehicle, or stand within 5 metres of it, or use /deletevehicle <vehicle id>.");
                return;
            }

            // Vehicles from /spawnveh have no row, so there is nothing to delete for good.
            VehicleDataComponent? vehicleDataComponent = vehicle.GetComponent<VehicleDataComponent>();
            if (vehicleDataComponent == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "This vehicle isn't saved (it came from /spawnveh), so it can't be deleted for good.");
                return;
            }

            ConfirmDelete(player, dataContext, vehicleDataComponent.VehicleData.Id);
        }

        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Delete a persistent vehicle for good.")]
        public void DeleteVehicle(Player player, BaseDataContext dataContext, int vehicleId)
        {
            if (vehicleId <= 0)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "Enter a vehicle id from 1 up.");
                return;
            }

            ConfirmDelete(player, dataContext, (ulong)vehicleId);
        }

        private void ConfirmDelete(Player player, BaseDataContext dataContext, ulong vehicleId)
        {
            VehicleModel? vehicleData = dataContext.Vehicles.AsNoTracking().FirstOrDefault(vehicle => vehicle.Id == vehicleId);
            if (vehicleData == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, $"There's no vehicle #{vehicleId}.");
                return;
            }

            string content = $"Delete vehicle #{vehicleId} ({_vehicleManager.DescribeModel(vehicleData)}, {_vehicleManager.GetNumberPlate(vehicleData)})? This can't be undone.";
            DeleteVehicleDialog.Open(player, _dialogService, content, () => DeleteConfirmed(player, vehicleId));
        }

        private void DeleteConfirmed(Player player, ulong vehicleId)
        {
            string? error = _vehicleManager.DeleteVehicle(vehicleId);
            if (error != null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
                return;
            }

            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"Vehicle #{vehicleId} is deleted.");
        }
    }
}
