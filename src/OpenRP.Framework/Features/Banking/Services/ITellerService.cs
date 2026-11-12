using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Banking.Enums;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Banking.Services
{
    /// <summary>
    /// Whether a teller can serve a customer. Every bank dialog checks this again at each step, because the teller can
    /// be moved, lose a permission or the customer can walk away while a dialog is open.
    /// </summary>
    public interface ITellerService
    {
        /// <summary>
        /// The closest bank teller standing near the player, or null.
        /// </summary>
        ServerActor? GetNearbyTeller(Player player);

        /// <summary>
        /// Why the teller can't do this for the customer right now, or null when it can.
        /// </summary>
        string? GetServiceError(ServerActor teller, Player customer, BankOperation operation);

        /// <summary>
        /// Whether the teller may do the operation, ignoring where the customer stands. For showing menu rows.
        /// </summary>
        bool IsAllowed(ServerActor teller, BankOperation operation);

        /// <summary>
        /// The bank the teller works for, or null.
        /// </summary>
        BankModel? GetBank(ServerActor teller);
    }
}
