using OpenRP.Framework.Features.Vehicles.Components;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Vehicles.Services
{
    public interface IVehicleEngineService
    {
        /// <summary>
        /// True while the vehicle's engine is being started.
        /// </summary>
        bool IsEngineStarting(Vehicle vehicle);

        /// <summary>
        /// Turns the key; the engine comes to a start a moment later (see EngineStartSystem).
        /// </summary>
        void BeginEngineStart(Player player, Vehicle vehicle);

        /// <summary>
        /// Starts the engine when the driver is still in the vehicle, and ends the start either way.
        /// </summary>
        void FinishEngineStart(EngineStarting engineStarting);

        /// <summary>
        /// Stops the engine right away.
        /// </summary>
        void StopEngine(Player player, Vehicle vehicle);
    }
}
