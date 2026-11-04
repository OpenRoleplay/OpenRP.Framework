using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Database.Services;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Currencies.Entities;
using OpenRP.Framework.Features.Currencies.Enums;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Items.Components;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using OpenRP.Framework.Shared.ServerEvents.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Currencies.Services
{
    public class CurrencyService : ICurrencyService
    {
        private const ulong CaeroynaRubleCurrencyId = 2;
        private const string CaeroynaRublePrefix = "CRU ";
        private static readonly CultureInfo MoneyCulture = CultureInfo.CreateSpecificCulture("en-US");

        private readonly IDataMemoryService _dataMemoryService;
        private readonly IEntityManager _entityManager;
        private readonly IServerEventAggregator _serverEventAggregator;
        private readonly ILogger<CurrencyService> _logger;

        public CurrencyService(IDataMemoryService dataMemoryService, IEntityManager entityManager, IServerEventAggregator serverEventAggregator, ILogger<CurrencyService> logger)
        {
            _dataMemoryService = dataMemoryService;
            _entityManager = entityManager;
            _serverEventAggregator = serverEventAggregator;
            _logger = logger;
        }

        public List<CurrencyModel> GetCurrencies()
        {
            return _dataMemoryService.GetCurrencies();
        }

        public CurrencyModel? GetCurrencyByCode(string currencyCode)
        {
            return GetCurrencies().FirstOrDefault(currency => string.Equals(currency.CurrencyCode, currencyCode, StringComparison.OrdinalIgnoreCase));
        }

        public void SendCurrencyCodes(Player player)
        {
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "Currency codes:");
            foreach (CurrencyModel currency in GetCurrencies())
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"{ChatColor.Highlight}{currency.CurrencyCode}{ChatColor.White} = {currency.Name}");
            }
        }

        public string GetCurrencyFormat(ulong currencyId, decimal amount)
        {
            string formattedAmount = amount.ToString("C", MoneyCulture);
            return currencyId == CaeroynaRubleCurrencyId ? formattedAmount.Replace("$", CaeroynaRublePrefix) : formattedAmount;
        }

        public decimal GetCharacterCurrencyAmount(Character character, ulong currencyId)
        {
            Inventory? wallet = character.GetWallet();
            if (wallet == null)
            {
                return 0;
            }
            return GetWalletBills(wallet, currencyId).Sum(bills => bills.GetValue());
        }

        public async Task<CurrencyChangeResult> GiveCharacterCurrency(Character character, ulong currencyId, decimal amount)
        {
            Inventory? wallet = character.GetWallet();
            if (wallet == null)
            {
                return CurrencyChangeResult.NoWallet;
            }

            List<BillStack> billsToAdd = GetBillsForAmount(currencyId, amount);
            if (GetTotalValue(billsToAdd) != amount)
            {
                return CurrencyChangeResult.AmountNotPayableInBills;
            }

            if (GetTotalWeight(billsToAdd) >= wallet.GetAvailableWeight() || !AddBillsToWallet(wallet, billsToAdd))
            {
                return CurrencyChangeResult.NotEnoughSpace;
            }

            await PublishCashUpdate(character, currencyId);
            return CurrencyChangeResult.Success;
        }

        public async Task<CurrencyChangeResult> TakeCharacterCurrency(Character character, ulong currencyId, decimal amount)
        {
            Inventory? wallet = character.GetWallet();
            if (wallet == null)
            {
                return CurrencyChangeResult.NoWallet;
            }

            if (GetCharacterCurrencyAmount(character, currencyId) < amount)
            {
                return CurrencyChangeResult.NotEnoughFunds;
            }

            List<BillStack> billsToRemove = GetBillsToPayAmount(wallet, currencyId, amount);
            decimal changeAmount = GetTotalValue(billsToRemove) - amount;
            List<BillStack> change = GetBillsForAmount(currencyId, changeAmount);
            if (GetTotalValue(change) != changeAmount)
            {
                return CurrencyChangeResult.AmountNotPayableInBills;
            }

            if (!DoesChangeFit(wallet, billsToRemove, change))
            {
                return CurrencyChangeResult.NotEnoughSpace;
            }

            if (!RemoveBillsFromWallet(wallet, billsToRemove) || !AddBillsToWallet(wallet, change))
            {
                _logger.LogError("Taking {Amount} of currency {CurrencyId} from character {CharacterId} only partly succeeded.", amount, currencyId, character.GetId());
                return CurrencyChangeResult.NotEnoughSpace;
            }

            await PublishCashUpdate(character, currencyId);
            return CurrencyChangeResult.Success;
        }

        public List<BillStack>? GetExactBillsFromWallet(Inventory wallet, ulong currencyId, decimal amount)
        {
            return FindExactBills(GetWalletBills(wallet, currencyId), 0, amount);
        }

        public bool TransferBills(Inventory fromWallet, Inventory toWallet, BillStack bills)
        {
            if (fromWallet.GetItemAmount(bills.Item) < bills.Amount || !toWallet.DoesItemFit(bills.Item, bills.Amount))
            {
                return false;
            }

            fromWallet.TakeItem(bills.Item, bills.Amount);
            toWallet.AddItem(bills.Item, bills.Amount);
            return true;
        }

        public void SetPlayerMoneyToCharacterCurrency(Player player, Character character)
        {
            ulong? defaultCurrencyId = character.GetPreferences()?.GetDefaultCurrencyId();
            if (defaultCurrencyId == null)
            {
                return;
            }

            decimal amount = GetCharacterCurrencyAmount(character, defaultCurrencyId.Value);
            player.ResetMoney();
            player.GiveMoney(Convert.ToInt32(Math.Floor(amount)));
        }

        private CurrencyUnitModel? GetCurrencyUnit(Item item)
        {
            ulong? currencyUnitId = item.GetCurrencyUnitId();
            if (currencyUnitId == null)
            {
                return null;
            }
            return _dataMemoryService.GetCurrencyUnits().FirstOrDefault(currencyUnit => currencyUnit.Id == currencyUnitId.Value);
        }

        private bool IsBillOfCurrency(Item item, ulong currencyId)
        {
            return GetCurrencyUnit(item)?.CurrencyId == currencyId;
        }

        private decimal GetBillValue(Item item)
        {
            return GetCurrencyUnit(item)?.UnitValue ?? 0;
        }

        /// <summary>
        /// The bill and coin items that exist for this currency, largest first.
        /// </summary>
        private List<Item> GetBillItems(ulong currencyId)
        {
            List<Item> billItems = _entityManager.GetComponents<Item>()
                .Where(item => IsBillOfCurrency(item, currencyId))
                .OrderByDescending(GetBillValue)
                .ToList();

            if (billItems.Count == 0)
            {
                _logger.LogWarning("Currency {CurrencyId} has no bill or coin items, so no money of it can be given or counted.", currencyId);
            }
            return billItems;
        }

        /// <summary>
        /// The bills of this currency in the wallet, one stack per denomination, largest first.
        /// </summary>
        private List<BillStack> GetWalletBills(Inventory wallet, ulong currencyId)
        {
            return GetBillItems(currencyId)
                .Select(item => new BillStack(item, wallet.GetItemAmount(item), GetBillValue(item)))
                .Where(bills => bills.Amount > 0)
                .ToList();
        }

        /// <summary>
        /// The fewest bills that make up this amount. Anything smaller than the smallest coin is left out.
        /// </summary>
        private List<BillStack> GetBillsForAmount(ulong currencyId, decimal amount)
        {
            List<BillStack> bills = new List<BillStack>();
            decimal amountLeft = amount;
            foreach (Item billItem in GetBillItems(currencyId))
            {
                decimal billValue = GetBillValue(billItem);
                uint billCount = (uint)Math.Floor(amountLeft / billValue);
                if (billCount == 0)
                {
                    continue;
                }

                bills.Add(new BillStack(billItem, billCount, billValue));
                amountLeft -= billCount * billValue;
            }
            return bills;
        }

        /// <summary>
        /// The bills to hand over for this amount: as much as possible without paying too much, plus the smallest
        /// larger bill when the rest can't be paid exactly. The wallet must hold at least the amount.
        /// </summary>
        private List<BillStack> GetBillsToPayAmount(Inventory wallet, ulong currencyId, decimal amount)
        {
            List<BillStack> walletBills = GetWalletBills(wallet, currencyId);
            List<BillStack> billsToPay = new List<BillStack>();
            decimal amountLeft = amount;

            foreach (BillStack bills in walletBills)
            {
                uint billCount = (uint)Math.Min(bills.Amount, Math.Floor(amountLeft / bills.UnitValue));
                if (billCount > 0)
                {
                    billsToPay.Add(bills with { Amount = billCount });
                    amountLeft -= billCount * bills.UnitValue;
                }
            }

            if (amountLeft > 0)
            {
                billsToPay.Add(GetSmallestBillToBreak(walletBills, billsToPay, amountLeft));
            }
            return MergeStacks(billsToPay);
        }

        /// <summary>
        /// The smallest bill still in the wallet that is worth more than the amount left to pay.
        /// </summary>
        private static BillStack GetSmallestBillToBreak(List<BillStack> walletBills, List<BillStack> billsAlreadyUsed, decimal amountLeft)
        {
            return walletBills
                .Where(bills => bills.UnitValue > amountLeft && bills.Amount > GetAmountUsed(billsAlreadyUsed, bills.Item))
                .OrderBy(bills => bills.UnitValue)
                .Select(bills => bills with { Amount = 1 })
                .First();
        }

        private static uint GetAmountUsed(List<BillStack> billsAlreadyUsed, Item item)
        {
            return (uint)billsAlreadyUsed.Where(bills => bills.Item == item).Sum(bills => bills.Amount);
        }

        private static List<BillStack> MergeStacks(List<BillStack> bills)
        {
            return bills
                .GroupBy(stack => stack.Item)
                .Select(group => group.First() with { Amount = (uint)group.Sum(stack => stack.Amount) })
                .ToList();
        }

        /// <summary>
        /// Searches the wallet's bills (largest first) for a combination worth exactly the amount.
        /// </summary>
        private static List<BillStack>? FindExactBills(List<BillStack> walletBills, int index, decimal amountLeft)
        {
            if (amountLeft == 0)
            {
                return new List<BillStack>();
            }

            if (index >= walletBills.Count)
            {
                return null;
            }

            BillStack bills = walletBills[index];
            uint maxBillCount = (uint)Math.Min(bills.Amount, Math.Floor(amountLeft / bills.UnitValue));
            for (long billCount = maxBillCount; billCount >= 0; billCount--)
            {
                List<BillStack>? rest = FindExactBills(walletBills, index + 1, amountLeft - billCount * bills.UnitValue);
                if (rest == null)
                {
                    continue;
                }

                if (billCount > 0)
                {
                    rest.Insert(0, bills with { Amount = (uint)billCount });
                }
                return rest;
            }
            return null;
        }

        private static decimal GetTotalValue(List<BillStack> bills)
        {
            return bills.Sum(stack => stack.GetValue());
        }

        private static uint GetTotalWeight(List<BillStack> bills)
        {
            return (uint)bills.Sum(stack => stack.GetWeight());
        }

        private static bool DoesChangeFit(Inventory wallet, List<BillStack> billsToRemove, List<BillStack> change)
        {
            return GetTotalWeight(change) < wallet.GetAvailableWeight() + GetTotalWeight(billsToRemove);
        }

        /// <summary>
        /// Adds the bills to the wallet. False when one of them didn't fit.
        /// </summary>
        private static bool AddBillsToWallet(Inventory wallet, List<BillStack> bills)
        {
            bool allAdded = true;
            foreach (BillStack stack in bills)
            {
                allAdded &= wallet.AddItem(stack.Item, stack.Amount);
            }
            return allAdded;
        }

        /// <summary>
        /// Takes the bills from the wallet. False when one of them wasn't there.
        /// </summary>
        private static bool RemoveBillsFromWallet(Inventory wallet, List<BillStack> bills)
        {
            bool allRemoved = true;
            foreach (BillStack stack in bills)
            {
                allRemoved &= wallet.TakeItem(stack.Item, stack.Amount);
            }
            return allRemoved;
        }

        private async Task PublishCashUpdate(Character character, ulong currencyId)
        {
            await _serverEventAggregator.PublishAsync(new OnPlayerCashUpdateEventArgs
            {
                Player = character.GetPlayer(),
                CurrencyId = currencyId,
            });
        }
    }
}
