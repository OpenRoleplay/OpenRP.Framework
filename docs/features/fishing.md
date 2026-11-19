# Fishing

Catch fish with a rod anywhere near water, or fit a Reefer with a net and trawl for a large catch. Your fishing skill decides how big a fish you can land and which treasures you can find.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/fish` | Default | Cast your line, while facing water or on a boat |
| `/fitnet` | Default | Fit your Fishing Net to the saved Reefer you are on or next to |
| `/removenet` | Default | Take the net off again, once its storage is empty |
| `/netstorage` | Default | Open the net's storage, on or next to the Reefer |
| `/trawl` | Default | Lower the net while driving a fitted Reefer, or raise it early |

**Gear:** a Fishing Rod (item 52) and Fishing Bait (53) for `/fish`, a Fishing Net (54) for `/fitnet`. Until shops exist, an admin hands these out with [`/giveitem`](admins.md):

| Gear | Command |
|---|---|
| Rod | `/giveitem [player] 52` |
| 20 bait | `/giveitem [player] 53 20` |
| Net | `/giveitem [player] 54` |

## Rod fishing

1. **Cast.** `/fish` uses 1 bait and 1 rod use. A rod lasts 50 casts.
2. **Wait.** A bite comes after 20 to 45 seconds, or half that near a [hotspot](#hotspots).
3. **Bite.** You have 3 seconds to press **Y**. Too late, and the fish gets away.
4. **Catch.** The catch goes into your inventory. If it doesn't fit, it lands at your feet.

Moving away, getting hurt, dying or disconnecting ends the cast. The bait and rod use are gone.

### What you can catch

Every bite lands something. Your fishing level, up to the [maximum of 100](skills.md), is a hard limit on size, and where you stand changes the odds.

| Size | Level needed | Odds from land | Odds from a boat | Species |
|---|---|---|---|---|
| Small | 0 | 100% | 100% | Sardine, Mackerel, Rainbow Trout, Swamp Eel, Mudfish, Ghost Carp |
| Medium | 5 | 60% | 100% | Largemouth Bass, Carp, Swamp Bass, Striped Bass |
| Large | 20 | 25% | 100% | California Halibut, Catfish, Mahi-Mahi |
| Huge | 50 | 5% | 100% | Bluefin Tuna |

- **Water.** Your position decides between salt, fresh and murky water, and each has its own species.
- **Junk,** in any water: Old Boot, Tin Can, Seaweed and, rarely (about 1%), a Rusty Paperclip.
- **Treasures,** in any water, with a rod only: Old Lockbox (level 10), Silver Ring (30), Gold Pocket Watch (60), Pirate Doubloon (90).
- **Weight.** Each fish rolls its own weight between the species' minimum and maximum.
- **Experience.** Bigger catches give more: junk 2, small 10, medium 25, large 60, huge 150, treasures 40.

**Why relative weights:** the old "1 in N" odds left 39 to 51% of casts with nothing. Now every bite pays off, and `CatchWeight` only decides which catch it is.

## Hotspots

30 hotspots sit on the water and move every 45 minutes. They have no map icon: you find them by their splash. Within 25 units, rod waits are halved and trawl progress runs 1.5 times as fast. They don't change what you can catch.

## Reefer net

Fitting a net needs a saved Reefer that you own or hold the key to. It gives the Reefer a storage of 500 kg that only accepts fish. Use `/netstorage` to move fish to your inventory, or to put your own fish in. Fish too heavy to carry stay in the storage. Deleting the Reefer deletes its net, its storage and the fish in it.

**Trawling:**

1. Drive a fitted Reefer on the water with the engine on, and use `/trawl`.
2. Sail at 5 to 20 km/h. Slower or faster pauses the progress, with a warning.
3. After 15 counted minutes the net comes up. Leaving the driver's seat or raising the net early ends the trawl with no catch.

The driver's fishing level decides the result, and the driver gets the experience:

| Result | Chance | Catch | Needs |
|---|---|---|---|
| Many small fish | 60% | 8 to 20 small fish | Level 0 |
| Fewer bigger fish | 30% | 2 to 5 medium or large fish | Level 5 for medium, 20 for large |
| One huge fish | 10% | One net-only fish of this water | The level of that species |

- **Level too low?** The next smaller result is used, so a trawl never comes up empty.
- **Net-only fish,** 25 to 150 kg: Swordfish (salt, level 60), Blue Marlin (salt, level 80), Giant Sturgeon (fresh, level 70), Alligator Gar (murky, level 70).
- **Storage full?** What doesn't fit is released, with a message.

**Why treasures are rod-only:** the net storage only holds fish.

## Known limitations

- **No shops yet.** Fish and treasures can't be sold, and rods, bait and nets can't be bought.
- **"Use" wears out a rod.** Choosing Use on a rod in the inventory costs one use, like any item with a use limit.
- **Fish lost at sea.** A catch that doesn't fit lands below you, which from a boat is the sea floor.
- **Long catches.** `/netstorage` lists the first 50 fish, because a dialog holds a limited amount of text.

## Deep dive

- **Code:** `Features/Fishing/`.
- **Tunable numbers:** `Entities/FishingSettings`.
- **Components:** `PlayerFishing` (a cast in progress), `PlayerTrawling` (a lowered net), `VehicleFishingNet` (a fitted net), `FishHotspot`.
- **Services:**
  - `IFishingService`: the rod loop;
  - `IFishService`: picks catches, rolls weights, gives experience, publishes [`OnFishCaught`](../server-events.md#onfishcaught);
  - `IFishHotspotService`: places and moves hotspots;
  - `IFishingNetService` and `IFishingNetManager`: fitted nets and their storage;
  - `ITrawlService`: the trawl;
  - `IFishingNetDialogService`: the storage dialog.
- **Water zones:** `Helpers/WaterTypeHelper`.
- **Skill:** the host seeds a skill named `Fishing`. Without it, players stay at level 0.
- **Tables:** [`FishLoot` and `VehicleFishingNets`](../database.md#fishing). The Framework seeds the catch table and its items.

Want more? → [Items](items.md) · [Skills](skills.md) · [Inventories](inventories.md)
