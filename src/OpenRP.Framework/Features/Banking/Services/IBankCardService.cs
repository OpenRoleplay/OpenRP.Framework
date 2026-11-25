using OpenRP.Framework.Features.Banking.Entities;
using OpenRP.Framework.Features.Banking.Enums;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Inventories.Components;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Banking.Services
{
    /// <summary>
    /// Debit cards: issuing them as items, PINs, blocking, names, and whether a card may be used. Methods that change
    /// something return why it didn't happen, or null.
    /// </summary>
    public interface IBankCardService
    {
        /// <summary>
        /// The card with its account and bank, read fresh, or null.
        /// </summary>
        BankCardDetails? GetCardDetails(ulong cardId);

        /// <summary>
        /// The character's card for the account that wasn't replaced, or null.
        /// </summary>
        BankCardDetails? GetCurrentCard(ulong accountId, ulong characterId);

        /// <summary>
        /// The card items the character carries, also those in wallets.
        /// </summary>
        List<InventoryItem> GetCarriedCardItems(Character character);

        /// <summary>
        /// Why the player can't use the card now, or null. The owner needs full access to the account; anyone else
        /// holding the card only needs the PIN, as with a real card.
        /// </summary>
        string? GetUseError(Player player, BankCardDetails card);

        /// <summary>
        /// A new card for the player's character and the account, as an item in their inventory. Their earlier card for
        /// that account is cancelled.
        /// </summary>
        Task<string?> IssueCardAsync(Player player, ulong accountId, string? name, string pin);

        /// <summary>
        /// Checks the PIN. A wrong one counts towards blocking the card; a correct one resets the count.
        /// </summary>
        Task<PinCheckResult> CheckPinAsync(ulong cardId, string pin);

        Task<string?> ChangePinAsync(ulong cardId, string pin);

        string? BlockCard(ulong cardId);

        /// <summary>
        /// Unblocks the card and resets the wrong PIN count. A cancelled card stays cancelled.
        /// </summary>
        string? UnblockCard(ulong cardId);

        /// <summary>
        /// Gives the card a name, or clears it with an empty name.
        /// </summary>
        string? RenameCard(ulong cardId, string name);

        /// <summary>
        /// Why the text isn't a valid PIN (exactly 4 digits), or null.
        /// </summary>
        string? GetPinError(string pin);

        /// <summary>
        /// Why the text isn't a valid card name, or null. An empty name is valid and clears the name.
        /// </summary>
        string? GetCardNameError(string name);
    }
}
