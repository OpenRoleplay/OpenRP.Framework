using OpenRP.Framework.Features.Phones.Components;
using OpenRP.Framework.Shared.Batteries.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Phones.Services
{
    /// <summary>
    /// Phone batteries: draining while in play, resting and recharging with an offline owner, and recharging on request.
    /// </summary>
    public interface IPhoneBatteryService
    {
        /// <summary>
        /// Brings every phone's battery up to date, with the low battery warnings and switching off at empty.
        /// </summary>
        void UpdateBatteries(BatteryPlaces places);

        /// <summary>
        /// Finishes recharges that are done, and stops those whose phone left the player who started them.
        /// </summary>
        void ProcessCharging();

        /// <summary>
        /// Starts recharging the phone. Returns why it can't, or null.
        /// </summary>
        string? BeginRecharge(Player player, Phone phone);

        /// <summary>
        /// The battery level now, also part-way through a recharge.
        /// </summary>
        double GetCurrentLevel(Phone phone);
    }
}
