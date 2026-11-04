using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Currencies.Services;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.ServerEvents.Attributes;
using OpenRP.Framework.Shared.ServerEvents.Entities;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;

namespace OpenRP.Framework.Features.Currencies.Systems
{
    /// <summary>
    /// Keeps the player's GTA money in line with the money in their character's wallet.
    /// </summary>
    public class CurrencyServerSystem : IServerSystem
    {
        [ServerEvent]
        public void OnPlayerCashUpdate(OnPlayerCashUpdateEventArgs args, ICurrencyService currencyService)
        {
            if (args.Player == null || !args.Player.IsPlayerPlayingAsCharacter())
            {
                return;
            }

            Character character = args.Player.GetPlayerCurrentlyPlayingAsCharacter();
            if (IsDefaultCurrency(character, args.CurrencyId))
            {
                currencyService.SetPlayerMoneyToCharacterCurrency(args.Player, character);
            }
        }

        [ServerEvent]
        public void OnCharacterSelected(OnCharacterSelectedEventArgs args, ICurrencyService currencyService)
        {
            if (args.Player == null || args.Character == null)
            {
                return;
            }

            currencyService.SetPlayerMoneyToCharacterCurrency(args.Player, args.Character);
        }

        private static bool IsDefaultCurrency(Character character, ulong currencyId)
        {
            return character.GetPreferences()?.GetDefaultCurrencyId() == currencyId;
        }
    }
}
