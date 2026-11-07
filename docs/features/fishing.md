# Fishing

An early version of fishing. `/fish` near water picks a random catch for the kind of water you're at, but only tells you what you caught.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/fish` | Default | Fish, while facing water |

## How it works

1. **Facing water?** ColAndreas checks whether you're facing water.
2. **Which water?** Your position decides on saltwater, freshwater or murky water.
3. **The catch.** A species for that water is picked by its odds (1 in N), and you get a message with its weight.

At startup, 30 fish **hotspots** are placed at random spots at sea.

## Known limitations

- **Unfinished:** no item, no rod, no waiting, and hotspots have no effect.
- **Test code.** Every hotspot shows as a map icon on connect (`FishHotspotSystem.OnPlayerConnect`).

## Deep dive

- **Code:** `Features/Fishing/`.
- **Services:**
  - `IFishService`: `GetRandomFishLoot` and `GetFishLootTypeZone`;
  - `IFishHotspotService`.
- **Water zones:** `Helpers/WaterTypeHelper`.
- **Table:** [`FishLoot`](../database.md#fishing). The host seeds the species, and the Framework seeds the 12 fish items.

Want more? → [Items](items.md)
