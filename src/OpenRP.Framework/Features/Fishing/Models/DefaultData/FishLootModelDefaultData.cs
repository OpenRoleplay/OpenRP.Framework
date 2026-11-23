using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Extensions;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Fishing.Entities;
using OpenRP.Framework.Features.Fishing.Enums;

namespace OpenRP.Framework.Features.Fishing.Models.DefaultData
{
    /// <summary>
    /// Everything a rod or net can bring up. CatchWeight is relative: a catch with weight 40 is twice as likely as one with weight 20.
    /// </summary>
    public class FishLootModelDefaultData : IEntityTypeConfiguration<FishLootModel>
    {
        public void Configure(EntityTypeBuilder<FishLootModel> builder)
        {
            builder.HasDefaultData(
                new FishLootModel()
                {
                    Id = 1,
                    Name = "California Halibut",
                    FishLootType = FishLootType.Saltwater,
                    LootKind = FishLootKind.Fish,
                    SizeClass = FishSizeClass.Large,
                    RequiredFishingLevel = FishingSettings.LargeFishRequiredLevel,
                    IsNetOnly = false,
                    MinWeightInGrams = 500,
                    MaxWeightInGrams = 3000,
                    CatchWeight = 25,
                    ItemId = 37
                },

                new FishLootModel()
                {
                    Id = 2,
                    Name = "Striped Bass",
                    FishLootType = FishLootType.Saltwater,
                    LootKind = FishLootKind.Fish,
                    SizeClass = FishSizeClass.Medium,
                    RequiredFishingLevel = FishingSettings.MediumFishRequiredLevel,
                    IsNetOnly = false,
                    MinWeightInGrams = 300,
                    MaxWeightInGrams = 2000,
                    CatchWeight = 40,
                    ItemId = 38
                },

                new FishLootModel()
                {
                    Id = 3,
                    Name = "Bluefin Tuna",
                    FishLootType = FishLootType.Saltwater,
                    LootKind = FishLootKind.Fish,
                    SizeClass = FishSizeClass.Huge,
                    RequiredFishingLevel = FishingSettings.HugeFishRequiredLevel,
                    IsNetOnly = false,
                    MinWeightInGrams = 2000,
                    MaxWeightInGrams = 10000,
                    CatchWeight = 10,
                    ItemId = 39
                },

                new FishLootModel()
                {
                    Id = 4,
                    Name = "Mahi-Mahi",
                    FishLootType = FishLootType.Saltwater,
                    LootKind = FishLootKind.Fish,
                    SizeClass = FishSizeClass.Large,
                    RequiredFishingLevel = FishingSettings.LargeFishRequiredLevel,
                    IsNetOnly = false,
                    MinWeightInGrams = 500,
                    MaxWeightInGrams = 2500,
                    CatchWeight = 25,
                    ItemId = 40
                },

                new FishLootModel()
                {
                    Id = 5,
                    Name = "Largemouth Bass",
                    FishLootType = FishLootType.Freshwater,
                    LootKind = FishLootKind.Fish,
                    SizeClass = FishSizeClass.Medium,
                    RequiredFishingLevel = FishingSettings.MediumFishRequiredLevel,
                    IsNetOnly = false,
                    MinWeightInGrams = 200,
                    MaxWeightInGrams = 1500,
                    CatchWeight = 40,
                    ItemId = 41
                },

                new FishLootModel()
                {
                    Id = 6,
                    Name = "Rainbow Trout",
                    FishLootType = FishLootType.Freshwater,
                    LootKind = FishLootKind.Fish,
                    SizeClass = FishSizeClass.Small,
                    RequiredFishingLevel = FishingSettings.SmallFishRequiredLevel,
                    IsNetOnly = false,
                    MinWeightInGrams = 100,
                    MaxWeightInGrams = 1000,
                    CatchWeight = 50,
                    ItemId = 42
                },

                new FishLootModel()
                {
                    Id = 7,
                    Name = "Catfish",
                    FishLootType = FishLootType.Freshwater,
                    LootKind = FishLootKind.Fish,
                    SizeClass = FishSizeClass.Large,
                    RequiredFishingLevel = FishingSettings.LargeFishRequiredLevel,
                    IsNetOnly = false,
                    MinWeightInGrams = 300,
                    MaxWeightInGrams = 2500,
                    CatchWeight = 30,
                    ItemId = 43
                },

                new FishLootModel()
                {
                    Id = 8,
                    Name = "Carp",
                    FishLootType = FishLootType.Freshwater,
                    LootKind = FishLootKind.Fish,
                    SizeClass = FishSizeClass.Medium,
                    RequiredFishingLevel = FishingSettings.MediumFishRequiredLevel,
                    IsNetOnly = false,
                    MinWeightInGrams = 100,
                    MaxWeightInGrams = 2000,
                    CatchWeight = 45,
                    ItemId = 44
                },

                new FishLootModel()
                {
                    Id = 9,
                    Name = "Swamp Eel",
                    FishLootType = FishLootType.Murkywater,
                    LootKind = FishLootKind.Fish,
                    SizeClass = FishSizeClass.Small,
                    RequiredFishingLevel = FishingSettings.SmallFishRequiredLevel,
                    IsNetOnly = false,
                    MinWeightInGrams = 50,
                    MaxWeightInGrams = 500,
                    CatchWeight = 40,
                    ItemId = 45
                },

                new FishLootModel()
                {
                    Id = 10,
                    Name = "Mudfish",
                    FishLootType = FishLootType.Murkywater,
                    LootKind = FishLootKind.Fish,
                    SizeClass = FishSizeClass.Small,
                    RequiredFishingLevel = FishingSettings.SmallFishRequiredLevel,
                    IsNetOnly = false,
                    MinWeightInGrams = 80,
                    MaxWeightInGrams = 600,
                    CatchWeight = 40,
                    ItemId = 46
                },

                new FishLootModel()
                {
                    Id = 11,
                    Name = "Ghost Carp",
                    FishLootType = FishLootType.Murkywater,
                    LootKind = FishLootKind.Fish,
                    SizeClass = FishSizeClass.Small,
                    RequiredFishingLevel = FishingSettings.SmallFishRequiredLevel,
                    IsNetOnly = false,
                    MinWeightInGrams = 100,
                    MaxWeightInGrams = 800,
                    CatchWeight = 20,
                    ItemId = 47
                },

                new FishLootModel()
                {
                    Id = 12,
                    Name = "Swamp Bass",
                    FishLootType = FishLootType.Murkywater,
                    LootKind = FishLootKind.Fish,
                    SizeClass = FishSizeClass.Medium,
                    RequiredFishingLevel = FishingSettings.MediumFishRequiredLevel,
                    IsNetOnly = false,
                    MinWeightInGrams = 150,
                    MaxWeightInGrams = 1200,
                    CatchWeight = 30,
                    ItemId = 48
                },

                new FishLootModel()
                {
                    Id = 13,
                    Name = "Rusty Paperclip",
                    FishLootType = FishLootType.Misc,
                    LootKind = FishLootKind.Junk,
                    SizeClass = FishSizeClass.None,
                    RequiredFishingLevel = 0,
                    IsNetOnly = false,
                    MinWeightInGrams = 5,
                    MaxWeightInGrams = 5,
                    CatchWeight = 1,
                    ItemId = 33
                },

                new FishLootModel()
                {
                    Id = 14,
                    Name = "Old Boot",
                    FishLootType = FishLootType.Misc,
                    LootKind = FishLootKind.Junk,
                    SizeClass = FishSizeClass.None,
                    RequiredFishingLevel = 0,
                    IsNetOnly = false,
                    MinWeightInGrams = 800,
                    MaxWeightInGrams = 800,
                    CatchWeight = 15,
                    ItemId = 55
                },

                new FishLootModel()
                {
                    Id = 15,
                    Name = "Tin Can",
                    FishLootType = FishLootType.Misc,
                    LootKind = FishLootKind.Junk,
                    SizeClass = FishSizeClass.None,
                    RequiredFishingLevel = 0,
                    IsNetOnly = false,
                    MinWeightInGrams = 100,
                    MaxWeightInGrams = 100,
                    CatchWeight = 15,
                    ItemId = 56
                },

                new FishLootModel()
                {
                    Id = 16,
                    Name = "Seaweed",
                    FishLootType = FishLootType.Misc,
                    LootKind = FishLootKind.Junk,
                    SizeClass = FishSizeClass.None,
                    RequiredFishingLevel = 0,
                    IsNetOnly = false,
                    MinWeightInGrams = 300,
                    MaxWeightInGrams = 300,
                    CatchWeight = 15,
                    ItemId = 57
                },

                new FishLootModel()
                {
                    Id = 17,
                    Name = "Old Lockbox",
                    FishLootType = FishLootType.Misc,
                    LootKind = FishLootKind.Treasure,
                    SizeClass = FishSizeClass.None,
                    RequiredFishingLevel = FishingSettings.OldLockboxRequiredLevel,
                    IsNetOnly = false,
                    MinWeightInGrams = 1500,
                    MaxWeightInGrams = 1500,
                    CatchWeight = 4,
                    ItemId = 58
                },

                new FishLootModel()
                {
                    Id = 18,
                    Name = "Silver Ring",
                    FishLootType = FishLootType.Misc,
                    LootKind = FishLootKind.Treasure,
                    SizeClass = FishSizeClass.None,
                    RequiredFishingLevel = FishingSettings.SilverRingRequiredLevel,
                    IsNetOnly = false,
                    MinWeightInGrams = 10,
                    MaxWeightInGrams = 10,
                    CatchWeight = 3,
                    ItemId = 59
                },

                new FishLootModel()
                {
                    Id = 19,
                    Name = "Gold Pocket Watch",
                    FishLootType = FishLootType.Misc,
                    LootKind = FishLootKind.Treasure,
                    SizeClass = FishSizeClass.None,
                    RequiredFishingLevel = FishingSettings.GoldPocketWatchRequiredLevel,
                    IsNetOnly = false,
                    MinWeightInGrams = 120,
                    MaxWeightInGrams = 120,
                    CatchWeight = 2,
                    ItemId = 60
                },

                new FishLootModel()
                {
                    Id = 20,
                    Name = "Pirate Doubloon",
                    FishLootType = FishLootType.Misc,
                    LootKind = FishLootKind.Treasure,
                    SizeClass = FishSizeClass.None,
                    RequiredFishingLevel = FishingSettings.PirateDoubloonRequiredLevel,
                    IsNetOnly = false,
                    MinWeightInGrams = 25,
                    MaxWeightInGrams = 25,
                    CatchWeight = 1,
                    ItemId = 61
                },

                new FishLootModel()
                {
                    Id = 21,
                    Name = "Blue Marlin",
                    FishLootType = FishLootType.Saltwater,
                    LootKind = FishLootKind.Fish,
                    SizeClass = FishSizeClass.Huge,
                    RequiredFishingLevel = FishingSettings.BlueMarlinRequiredLevel,
                    IsNetOnly = true,
                    MinWeightInGrams = 40000,
                    MaxWeightInGrams = 150000,
                    CatchWeight = 1,
                    ItemId = 62
                },

                new FishLootModel()
                {
                    Id = 22,
                    Name = "Swordfish",
                    FishLootType = FishLootType.Saltwater,
                    LootKind = FishLootKind.Fish,
                    SizeClass = FishSizeClass.Huge,
                    RequiredFishingLevel = FishingSettings.SwordfishRequiredLevel,
                    IsNetOnly = true,
                    MinWeightInGrams = 25000,
                    MaxWeightInGrams = 100000,
                    CatchWeight = 1,
                    ItemId = 63
                },

                new FishLootModel()
                {
                    Id = 23,
                    Name = "Giant Sturgeon",
                    FishLootType = FishLootType.Freshwater,
                    LootKind = FishLootKind.Fish,
                    SizeClass = FishSizeClass.Huge,
                    RequiredFishingLevel = FishingSettings.GiantSturgeonRequiredLevel,
                    IsNetOnly = true,
                    MinWeightInGrams = 30000,
                    MaxWeightInGrams = 120000,
                    CatchWeight = 1,
                    ItemId = 64
                },

                new FishLootModel()
                {
                    Id = 24,
                    Name = "Alligator Gar",
                    FishLootType = FishLootType.Murkywater,
                    LootKind = FishLootKind.Fish,
                    SizeClass = FishSizeClass.Huge,
                    RequiredFishingLevel = FishingSettings.AlligatorGarRequiredLevel,
                    IsNetOnly = true,
                    MinWeightInGrams = 25000,
                    MaxWeightInGrams = 90000,
                    CatchWeight = 1,
                    ItemId = 65
                },

                new FishLootModel()
                {
                    Id = 25,
                    Name = "Sardine",
                    FishLootType = FishLootType.Saltwater,
                    LootKind = FishLootKind.Fish,
                    SizeClass = FishSizeClass.Small,
                    RequiredFishingLevel = FishingSettings.SmallFishRequiredLevel,
                    IsNetOnly = false,
                    MinWeightInGrams = 30,
                    MaxWeightInGrams = 150,
                    CatchWeight = 30,
                    ItemId = 66
                },

                new FishLootModel()
                {
                    Id = 26,
                    Name = "Mackerel",
                    FishLootType = FishLootType.Saltwater,
                    LootKind = FishLootKind.Fish,
                    SizeClass = FishSizeClass.Small,
                    RequiredFishingLevel = FishingSettings.SmallFishRequiredLevel,
                    IsNetOnly = false,
                    MinWeightInGrams = 100,
                    MaxWeightInGrams = 600,
                    CatchWeight = 25,
                    ItemId = 67
                }
            );
        }
    }
}
