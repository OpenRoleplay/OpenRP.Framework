using OpenRP.Framework.Features.Vehicles.Enums;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Vehicles.Services
{
    public interface IVehicleIndicatorService
    {
        /// <summary>
        /// True when the vehicle can have turn signals: its model has light panels and the model has size data.
        /// </summary>
        bool HasTurnSignals(Vehicle vehicle);

        /// <summary>
        /// Which turn signals of the vehicle are on, Off when none are.
        /// </summary>
        IndicatorState GetIndicatorState(Vehicle vehicle);

        /// <summary>
        /// Switches the vehicle's turn signals to the state and sends the driver's /me line. Returns why it was refused, or null when it worked.
        /// </summary>
        string? SetIndicator(Player driver, Vehicle vehicle, IndicatorState state);

        /// <summary>
        /// Applies a look key press by the driver, through the rules in IndicatorRules. Does nothing while the driver's key
        /// cooldown runs, or when the press wouldn't change the signals. Otherwise it changes them, starts the cooldown and sends the /me line.
        /// </summary>
        void SetIndicatorFromKeyPress(Player driver, Vehicle vehicle, IndicatorState pressedState);

        /// <summary>
        /// Turns off the left and right turn signals of every vehicle this player was driving, when they leave the driver seat
        /// or disconnect. Hazards stay on, as they do in a real car. Sends no message.
        /// </summary>
        void TurnOffTurnSignalsDrivenBy(EntityId driverEntityId);

        /// <summary>
        /// Removes the player's key cooldown, when they disconnect.
        /// </summary>
        void RemoveKeyCooldown(Player player);
    }
}
