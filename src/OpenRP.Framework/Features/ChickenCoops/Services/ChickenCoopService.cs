using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Features.CDN.Services;
using OpenRP.Framework.Features.ChickenCoops.Components;
using OpenRP.Framework.Features.ChickenCoops.Entities;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Items.Components;
using OpenRP.Framework.Features.Items.Entities;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.Chat.Services;
using OpenRP.OpenMp.Streamer;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Linq;

namespace OpenRP.Framework.Features.ChickenCoops.Services
{
    public class ChickenCoopService : IChickenCoopService
    {
        private const int EggItemId = 36;
        private const int MessageStepCount = 3;
        private const float LabelDrawDistance = 3.0f;
        private const float SoundRange = 3.0f;
        private const string SoundDirectory = "sfx";
        private const string ChickenActivitySound = "chickenActivity.mp3";
        private const string EggLaidSound = "chickenEggPop.mp3";
        private const string AlarmSound = "chickenAlarmCall.mp3";
        private const string FailedMessage = "You failed to collect an egg without disturbing the coop.";
        private static readonly Color LabelColor = new Color(246, 165, 250);

        private readonly IEntityManager _entityManager;
        private readonly IStreamerService _streamerService;
        private readonly IChatService _chatService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ChickenCoopService> _logger;
        private readonly Random _random = new Random();

        public ChickenCoopService(IEntityManager entityManager, IStreamerService streamerService, IChatService chatService, IServiceProvider serviceProvider, ILogger<ChickenCoopService> logger)
        {
            _entityManager = entityManager;
            _streamerService = streamerService;
            _chatService = chatService;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public ChickenCoop CreateChickenCoop(Vector3 position)
        {
            EntityId entityId = ChickenCoopEntities.GenerateChickenCoopId();
            ChickenCoop coop = _entityManager.AddComponent<ChickenCoop>(entityId, position);
            _streamerService.CreateDynamicTextLabel(coop.GetLabelText(), LabelColor, position, LabelDrawDistance, parent: entityId);
            return coop;
        }

        public void BeginCollectEgg(Player player)
        {
            ChickenCoop? coop = GetNearbyCoop(player);
            string? error = GetBeginCollectEggError(player, coop);
            if (error != null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
                return;
            }

            player.AddComponent<PlayerCollectingEgg>(coop!.Entity, IsCrouching(player));
        }

        public void ContinueEggCollections()
        {
            foreach (PlayerCollectingEgg collecting in _entityManager.GetComponents<PlayerCollectingEgg>().Where(collecting => collecting.IsNextStepDue()).ToList())
            {
                ContinueEggCollection(collecting);
            }
        }

        public void UpdateChickenCoops()
        {
            foreach (ChickenCoop coop in _entityManager.GetComponents<ChickenCoop>().ToList())
            {
                if (coop.BreedChickenIfDue())
                {
                    PlayCoopSound(coop, ChickenActivitySound);
                }
                if (coop.LayEggsIfDue(_random) > 0)
                {
                    PlayCoopSound(coop, EggLaidSound);
                }
            }
        }

        private string? GetBeginCollectEggError(Player player, ChickenCoop? coop)
        {
            if (!player.IsPlayerPlayingAsCharacter())
                return "You must be playing as a character in order to do this command!";
            if (player.GetComponent<PlayerCollectingEgg>() != null)
                return "You are already collecting an egg.";
            if (coop == null)
                return "You must be near a chicken coop to do this command!";
            if (GetEggItem() == null)
                return "Eggs can't be collected right now.";
            if (!HasInventorySpaceForEgg(player))
                return "You do not have enough space in your inventory for this item!";
            return null;
        }

        private void ContinueEggCollection(PlayerCollectingEgg collecting)
        {
            Player player = collecting.GetComponent<Player>();
            ChickenCoop? coop = _entityManager.GetComponent<ChickenCoop>(collecting.GetCoopEntityId());
            if (coop == null || !coop.IsPlayerNearby(player) || !player.IsPlayerPlayingAsCharacter())
            {
                EndCollection(collecting, "You are no longer near the chicken coop!");
                return;
            }

            if (collecting.IsCareful() && !IsCrouching(player))
            {
                ScareChickens(collecting, coop);
                return;
            }

            if (collecting.GetStep() < MessageStepCount)
            {
                SendStepMessage(player, collecting, coop);
                collecting.AdvanceStep();
                return;
            }

            FinishCollection(collecting, coop);
        }

        private void SendStepMessage(Player player, PlayerCollectingEgg collecting, ChickenCoop coop)
        {
            (PlayerChatMessageType type, string text) = collecting.IsCareful()
                ? GetCarefulStepMessage(collecting.GetStep(), coop.HasEggs())
                : GetStandingStepMessage(collecting.GetStep());
            _chatService.SendPlayerChatMessage(player, type, text);
        }

        private static (PlayerChatMessageType, string) GetCarefulStepMessage(int step, bool hasEggs)
        {
            return step switch
            {
                0 => (PlayerChatMessageType.ME, "carefully reaches into the chicken coop."),
                1 => (PlayerChatMessageType.DO, "The chickens cluck softly as you handle them gently."),
                _ => (PlayerChatMessageType.ME, hasEggs ? "finds a warm egg nestled in the hay." : "finds no eggs nestled in the hay."),
            };
        }

        private static (PlayerChatMessageType, string) GetStandingStepMessage(int step)
        {
            return step switch
            {
                0 => (PlayerChatMessageType.ME, "reaches into the chicken coop whilst standing, unable to see inside the chicken coop."),
                1 => (PlayerChatMessageType.DO, "A feisty chicken pecks aggressively at your hand!"),
                _ => (PlayerChatMessageType.ME, "yelps and pulls their hand back quickly, startled by the chicken."),
            };
        }

        private void ScareChickens(PlayerCollectingEgg collecting, ChickenCoop coop)
        {
            Player player = collecting.GetComponent<Player>();
            _chatService.SendPlayerChatMessage(player, PlayerChatMessageType.DO, "The chickens scatter nervously as you stand up, making it impossible to collect any eggs.");
            EndCollection(collecting, FailedMessage);
            PlayCoopSound(coop, AlarmSound);
        }

        private void FinishCollection(PlayerCollectingEgg collecting, ChickenCoop coop)
        {
            if (!collecting.IsCareful())
            {
                EndCollection(collecting, FailedMessage);
                PlayCoopSound(coop, AlarmSound);
                return;
            }

            Player player = collecting.GetComponent<Player>();
            if (!coop.HasEggs())
            {
                EndCollection(collecting, "There are currently no eggs in the chicken coop.");
                return;
            }
            if (!AddEggToInventory(player))
            {
                EndCollection(collecting, "You do not have enough space in your inventory for this item!");
                return;
            }

            coop.RemoveOneEgg();
            collecting.Destroy();
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "You successfully collected an egg without disturbing the coop.");
        }

        private static void EndCollection(PlayerCollectingEgg collecting, string errorMessage)
        {
            Player player = collecting.GetComponent<Player>();
            collecting.Destroy();
            player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, errorMessage);
        }

        private bool AddEggToInventory(Player player)
        {
            return HasInventorySpaceForEgg(player) && GetInventory(player)!.AddItem(GetEggItem()!, 1);
        }

        private bool HasInventorySpaceForEgg(Player player)
        {
            Inventory? inventory = GetInventory(player);
            Item? eggItem = GetEggItem();
            return inventory != null && eggItem != null && inventory.DoesItemFit(eggItem, 1);
        }

        private static Inventory? GetInventory(Player player)
        {
            return player.GetPlayerCurrentlyPlayingAsCharacter()?.GetInventory();
        }

        private Item? GetEggItem()
        {
            Item? eggItem = _entityManager.GetComponent<Item>(ItemEntities.GetItemId(EggItemId));
            if (eggItem == null)
            {
                _logger.LogError("The egg item (id {ItemId}) isn't loaded, so eggs can't be collected.", EggItemId);
            }
            return eggItem;
        }

        private ChickenCoop? GetNearbyCoop(Player player)
        {
            return _entityManager.GetComponents<ChickenCoop>().FirstOrDefault(coop => coop.IsPlayerNearby(player));
        }

        private static bool IsCrouching(Player player)
        {
            return player.SpecialAction == SpecialAction.Duck;
        }

        /// <summary>
        /// Sounds come from the CDN feature, which a host may not have added; the coop works without them.
        /// </summary>
        private void PlayCoopSound(ChickenCoop coop, string sound)
        {
            IOpenCdnService? openCdnService = _serviceProvider.GetService<IOpenCdnService>();
            if (openCdnService == null)
            {
                return;
            }

            foreach (Player player in _entityManager.GetComponents<Player>())
            {
                openCdnService.Play(player, SoundDirectory, sound, coop.GetPosition(), SoundRange);
            }
        }
    }
}
