using OpenRP.Framework.Features.Radios.Components;
using OpenRP.Framework.Shared.Batteries.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Radios.Services
{
    /// <summary>
    /// Handheld radio batteries: draining while in play, resting and recharging with an offline owner, and recharging
    /// on request. Vehicle radios run on the car and have none.
    /// </summary>
    public interface IRadioBatteryService
    {
        /// <summary>
        /// Brings every handheld's battery up to date, with the low battery warnings and switching off at empty.
        /// </summary>
        void UpdateBatteries(BatteryPlaces places);

        /// <summary>
        /// Finishes recharges that are done, and stops those whose radio left the player who started them.
        /// </summary>
        void ProcessCharging();

        /// <summary>
        /// Starts recharging the handheld. Returns why it can't, or null.
        /// </summary>
        string? BeginRecharge(Player player, Radio radio);

        /// <summary>
        /// The battery level now, also part-way through a recharge.
        /// </summary>
        double GetCurrentLevel(Radio radio);

        /// <summary>
        /// The extra drain of transmitting once with a handheld.
        /// </summary>
        void UseTransmitBattery(Player holder, Radio radio);
    }
}
