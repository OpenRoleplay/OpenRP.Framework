using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Items.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Items.Models.DefaultData
{
    public class ItemModelDefaultData : IEntityTypeConfiguration<ItemModel>
    {
        public void Configure(EntityTypeBuilder<ItemModel> builder)
        {
            builder.HasData(
                new ItemModel
                {
                    Id = 1
                    , Name = "Kir Jong Uniform"
                    , Description = "One-size-fits-all authoritarian regimes and questionable morals."
                    , UseType = ItemType.Skin
                    , Weight = 500
                    , CanDrop = false
                    , CanDestroy = true
                    , KeepOnDeath = true
                    , UseValue = "[SKIN=295]"
                },
                new ItemModel
                {
                    Id = 2
                    , Name = "Wallet"
                    , Description = "Contains hopes, dreams, and exactly zero dollars."
                    , UseType = ItemType.Wallet
                    , Weight = 25
                    , CanDrop = true
                    , CanDestroy = true
                    , KeepOnDeath = true
                },
                new ItemModel
                {
                    Id = 3,
                    Name = "San Andreas Dollar ($0.01)",
                    Description = "Grind hard, stack these, and one day you'll own a rusty old car.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=1]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 4,
                    Name = "San Andreas Dollar ($0.05)",
                    Description = "Grind hard, stack these, and one day you'll own a rusty old car.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=2]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 5,
                    Name = "San Andreas Dollar ($0.10)",
                    Description = "Grind hard, stack these, and one day you'll own a rusty old car.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=3]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 6,
                    Name = "San Andreas Dollar ($0.25)",
                    Description = "Grind hard, stack these, and one day you'll own a rusty old car.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=4]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 7,
                    Name = "San Andreas Dollar ($0.50)",
                    Description = "Grind hard, stack these, and one day you'll own a rusty old car.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=5]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 8,
                    Name = "San Andreas Dollar ($1.00)",
                    Description = "Grind hard, stack these, and one day you'll own a rusty old car.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=6]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 9,
                    Name = "San Andreas Dollar ($2.00)",
                    Description = "Grind hard, stack these, and one day you'll own a rusty old car.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=7]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 10,
                    Name = "San Andreas Dollar ($5.00)",
                    Description = "Grind hard, stack these, and one day you'll own a rusty old car.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=8]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 11,
                    Name = "San Andreas Dollar ($10.00)",
                    Description = "Grind hard, stack these, and one day you'll own a rusty old car.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=9]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 12,
                    Name = "San Andreas Dollar ($20.00)",
                    Description = "Grind hard, stack these, and one day you'll own a rusty old car.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=10]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 13,
                    Name = "San Andreas Dollar ($50.00)",
                    Description = "Grind hard, stack these, and one day you'll own a rusty old car.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=11]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 14,
                    Name = "San Andreas Dollar ($100.00)",
                    Description = "Grind hard, stack these, and one day you'll own a rusty old car.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=12]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 15,
                    Name = "San Andreas Dollar ($500.00)",
                    Description = "Grind hard, stack these, and one day you'll own a rusty old car.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=13]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 16,
                    Name = "San Andreas Dollar ($1,000.00)",
                    Description = "Grind hard, stack these, and one day you'll own a rusty old car.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=14]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 17,
                    Name = "San Andreas Dollar ($10,000.00)",
                    Description = "Grind hard, stack these, and one day you'll own a rusty old car.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=15]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                // Caeroyna Ruble Items
                new ItemModel
                {
                    Id = 18,
                    Name = "Caeroyna Ruble (CRU 0.01)",
                    Description = "In Caeroyna, it’s not about how many you have, but who you bribe with them.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=16]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 19,
                    Name = "Caeroyna Ruble (CRU 0.05)",
                    Description = "In Caeroyna, it’s not about how many you have, but who you bribe with them.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=17]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 20,
                    Name = "Caeroyna Ruble (CRU 0.10)",
                    Description = "In Caeroyna, it’s not about how many you have, but who you bribe with them.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=18]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 21,
                    Name = "Caeroyna Ruble (CRU 0.50)",
                    Description = "In Caeroyna, it’s not about how many you have, but who you bribe with them.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=19]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 22,
                    Name = "Caeroyna Ruble (CRU 1.00)",
                    Description = "In Caeroyna, it’s not about how many you have, but who you bribe with them.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=20]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 23,
                    Name = "Caeroyna Ruble (CRU 2.00)",
                    Description = "In Caeroyna, it’s not about how many you have, but who you bribe with them.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=21]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 24,
                    Name = "Caeroyna Ruble (CRU 5.00)",
                    Description = "In Caeroyna, it’s not about how many you have, but who you bribe with them.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=22]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 25,
                    Name = "Caeroyna Ruble (CRU 10.00)",
                    Description = "In Caeroyna, it’s not about how many you have, but who you bribe with them.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=23]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 26,
                    Name = "Caeroyna Ruble (CRU 50.00)",
                    Description = "In Caeroyna, it’s not about how many you have, but who you bribe with them.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=24]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 27,
                    Name = "Caeroyna Ruble (CRU 100.00)",
                    Description = "In Caeroyna, it’s not about how many you have, but who you bribe with them.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=25]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 28,
                    Name = "Caeroyna Ruble (CRU 200.00)",
                    Description = "In Caeroyna, it’s not about how many you have, but who you bribe with them.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=26]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 29,
                    Name = "Caeroyna Ruble (CRU 500.00)",
                    Description = "In Caeroyna, it’s not about how many you have, but who you bribe with them.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=27]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 30,
                    Name = "Caeroyna Ruble (CRU 1,000.00)",
                    Description = "In Caeroyna, it’s not about how many you have, but who you bribe with them.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=28]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 31,
                    Name = "Caeroyna Ruble (CRU 5,000.00)",
                    Description = "In Caeroyna, it’s not about how many you have, but who you bribe with them.",
                    UseType = ItemType.Currency,
                    UseValue = "[CURRENCY_UNIT=29]",
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    DropModelId = 1212,
                },
                new ItemModel
                {
                    Id = 32,
                    Name = "Vehicle Key",
                    Description = "The only thing standing between you and grand theft auto charges.",
                    UseType = ItemType.VehicleKey,
                    UseValue = String.Empty,
                    Weight = 1,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true
                },
                new ItemModel
                {
                    Id = 33,
                    Name = "Rusty Paperclip",
                    Description = "This battered paperclip might look like trash, but in San Andreas it’s the little tool that can hack your ride faster than you can say 'get outta here!'",
                    UseType = ItemType.Misc,
                    UseValue = String.Empty,
                    Weight = 5,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    MaxUses = 1,
                },
                new ItemModel
                {
                    Id = 34,
                    Name = "Worn-Out Screwdriver",
                    Description = "A screwdriver so beaten up it probably helped crack a few cases in Grove Street. It may not be pretty, but it gets the job done when you need to spark life into a car without a key.",
                    UseType = ItemType.Misc,
                    UseValue = String.Empty,
                    Weight = 60,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true
                },
                new ItemModel
                {
                    Id = 35,
                    Name = "Roll of Electrical Tape",
                    Description = "Sticky, weathered electrical tape that’s seen more close calls than a low-rider on a bad day. It’ll hold your hotwire setup together, until the cops show up, that is!",
                    UseType = ItemType.Misc,
                    UseValue = String.Empty,
                    Weight = 100,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    Stackable = false,
                    MaxUses = 5,
                    DropModelId = 19873
                },
                new ItemModel
                {
                    Id = 36,
                    Name = "Chicken Egg",
                    Description = "Fresh from the coop! Handle with care-one wrong move, and it's omelette time.",
                    UseType = ItemType.Misc,
                    UseValue = String.Empty,
                    Weight = 50,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true
                },
                new ItemModel
                {
                    Id = 37,
                    Name = "California Halibut",
                    Description = "This fish is as chill as a lowrider cruising the Pacific coast. The California Halibut doesn’t rush, it's more about that laid-back, SoCal vibe, even underwater.",
                    UseType = ItemType.Fish,
                    UseValue = "[FISH_SPECIES=1]",
                    Weight = 3000,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    Stackable = false,
                    DropModelId = 19630
                },
                new ItemModel
                {
                    Id = 38,
                    Name = "Striped Bass",
                    Description = "Rocking stripes like a pimp's suit, the Striped Bass struts through the water with more swagger than a Grove Street hustler. Catch it if you can, it's got attitude for days.",
                    UseType = ItemType.Fish,
                    UseValue = "[FISH_SPECIES=2]",
                    Weight = 2000,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    Stackable = false,
                    DropModelId = 19630
                },
                new ItemModel
                {
                    Id = 39,
                    Name = "Bluefin Tuna",
                    Description = "The heavyweight champ of the fish world, Bluefin Tuna is the muscle of the sea. Tough as a street brawler and twice as flashy, it’s the kind of catch that makes you feel like a boss.",
                    UseType = ItemType.Fish,
                    UseValue = "[FISH_SPECIES=3]",
                    Weight = 10000,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    Stackable = false,
                    DropModelId = 19630
                },
                new ItemModel
                {
                    Id = 40,
                    Name = "Mahi-Mahi",
                    Description = "Also known as the party fish, Mahi-Mahi is the cousin you never expected, loud, colorful, and always ready to make a splash at your underwater block party. It's fish with flair!",
                    UseType = ItemType.Fish,
                    UseValue = "[FISH_SPECIES=4]",
                    Weight = 2500,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    Stackable = false,
                    DropModelId = 19630
                },
                new ItemModel
                {
                    Id = 41,
                    Name = "Largemouth Bass",
                    Description = "With a jaw that could rival any notorious hustler's, the Largemouth Bass is the aquatic equivalent of that one relentless friend who just won't let go. It bites hard and makes a statement.",
                    UseType = ItemType.Fish,
                    UseValue = "[FISH_SPECIES=5]",
                    Weight = 1500,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    Stackable = false,
                    DropModelId = 19630
                },
                new ItemModel
                {
                    Id = 42,
                    Name = "Rainbow Trout",
                    Description = "Sporting colors brighter than a neon strip club sign, Rainbow Trout is all about style. It’s the fish that proves even the underdogs can rock the runway, if the runway were a river.",
                    UseType = ItemType.Fish,
                    UseValue = "[FISH_SPECIES=6]",
                    Weight = 1000,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    Stackable = false,
                    DropModelId = 19630
                },
                new ItemModel
                {
                    Id = 43,
                    Name = "Catfish",
                    Description = "Not to be confused with your average alleyway trickster, this Catfish prowls the murky depths like a seasoned street-smart hustler. Slick, sneaky, and always ready to pull a fast one on your bait.",
                    UseType = ItemType.Fish,
                    UseValue = "[FISH_SPECIES=7]",
                    Weight = 2500,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    Stackable = false,
                    DropModelId = 19630
                },
                new ItemModel
                {
                    Id = 44,
                    Name = "Carp",
                    Description = "The everyday warrior of the water world, Carp is as common as a corner store con and just as persistent. It might not be glamorous, but it’s got the hustle and grit of a true San Andreas survivor.",
                    UseType = ItemType.Fish,
                    UseValue = "[FISH_SPECIES=8]",
                    Weight = 2000,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    Stackable = false,
                    DropModelId = 19630
                },
                new ItemModel
                {
                    Id = 45,
                    Name = "Swamp Eel",
                    Description = "Think of the Swamp Eel as the secret agent of the bayou: slippery, mysterious, and always lurking in the dark. It glides through the muck like it’s on a covert mission for the boss.",
                    UseType = ItemType.Fish,
                    UseValue = "[FISH_SPECIES=9]",
                    Weight = 500,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    Stackable = false,
                    DropModelId = 19630
                },
                new ItemModel
                {
                    Id = 46,
                    Name = "Mudfish",
                    Description = "Straight outta the gutter, Mudfish embraces its messy origins. It’s as unpolished and unpredictable as a back-alley deal gone sideways, yet somehow it’s got that raw, street-level charm.",
                    UseType = ItemType.Fish,
                    UseValue = "[FISH_SPECIES=10]",
                    Weight = 500,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    Stackable = false,
                    DropModelId = 19630
                },
                new ItemModel
                {
                    Id = 47,
                    Name = "Ghost Carp",
                    Description = "The phantom prankster of the pond, Ghost Carp appears when you least expect it, mocking your fishing skills with an eerie, translucent grin. It’s spooky, surreal, and just a little bit hilarious.",
                    UseType = ItemType.Fish,
                    UseValue = "[FISH_SPECIES=11]",
                    Weight = 800,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    Stackable = false,
                    DropModelId = 19630
                },
                new ItemModel
                {
                    Id = 48,
                    Name = "Swamp Bass",
                    Description = "The enforcer of the murk, Swamp Bass rules the swamp like a grizzled OG. With a no-nonsense attitude and a reputation for causing a ruckus among the lily pads, it’s the heavyweight you don’t wanna mess with.",
                    UseType = ItemType.Fish,
                    UseValue = "[FISH_SPECIES=12]",
                    Weight = 1200,
                    CanDrop = true,
                    CanDestroy = true,
                    KeepOnDeath = true,
                    Stackable = false,
                    DropModelId = 19630
                }
            );
        }
    }
}
