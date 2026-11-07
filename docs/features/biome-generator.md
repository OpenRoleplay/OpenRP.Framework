# BiomeGenerator

The building blocks of Open Roleplay's procedural landscapes:
- **biomes** say what grows where;
- **object generators** say what a tree, bush or rock looks like.

The generator that places them is closed-source and lives in the official gamemode.

## How it works

1. **Regions.** The map is split into **biome types** (regions), such as `RedCounty`, `Desert` or `Fogbelt`.
2. **Biomes.** Each region has one or more **biomes**, such as "Oak Forest" or "Desert Cactus". Each has a spawn frequency (0–255).
3. **One spot.** For each spot, the biome picks what to place by weighted chance; usually "Nothing" is the most likely. A **generator** then builds that object: model, rotation, colour.

## How to add one

| To add | Do |
|---|---|
| An object | Implement `IBiomeObjectGenerator` with a new `ObjectType`, and register it in `AddBiomeGenerator()` |
| A biome | Implement `IBiome` with `[Biome(frequency, name, BiomeType)]` and a `WeightedRandom` of object types, and register it |
| A harvestable object | Also register a [harvestable model](harvestables.md) for its model id |

## Deep dive

- **Code:**
  - `Features/BiomeGenerator/`;
  - biomes in `Services/Generators/Biomes/`;
  - generators in `Services/Generators/Objects/`.
- **Noise:** `Shared/FastNoiseLite.cs`.
- **Biome types:**
  - `RedCounty`, `Panopticon`, `DeadForest`, `Farmland`, `Plains`;
  - `FlintWhetstoneCountyMain`, `AngelPineMain`, `FortCarsonMain`;
  - `LosSantosMain`, `LosSantosPlains`, `SanFierroMain`;
  - `Desert`, `DesertWastelands`, `DesertRestrictedZone`;
  - `Shrublands`, `Savanna`, `Fogbelt`;
  - `LasVenturasGravel`, `LasVenturasStrip`.

**The biomes, with their region and frequency:**

| Biome | Region (frequency) |
|---|---|
| Birch Forest | RedCounty 30, SanFierroMain 70 |
| Cedar Forest | FortCarsonMain 40 |
| Coniferous Woodland | Panopticon 255 |
| Dead Elm Forest | Shrublands 200 |
| Dead Forest (+ With Trees) | DeadForest 255 |
| Dense Fir Forest | FlintWhetstoneCountyMain 58 |
| Desert Cactus / Locust Forest / Palms / Plains | Desert and DesertRestrictedZone: 75 / 40 / 40 / 100 |
| Desert Wasteland Plains | DesertWastelands 255 |
| Elm Forest | SanFierroMain 50 |
| Farmland | Farmland 255 |
| Fogbelt Elm / Redwood Forest | Fogbelt 100 / 155 |
| Forest Plains | RedCounty 10 |
| Las Venturas Gravel | LasVenturasGravel 10 |
| Oak Forest | RedCounty 57, SanFierroMain 35 |
| Palm Beach / Palm Plains | LosSantosMain 30 / LosSantosPlains 30 |
| Plains | Plains 10 |
| Redwood / Sparse Redwood Forest | SanFierroMain 100 / Savanna 55 |
| Sparse Elm Forest | Savanna 200 |
| Sunflower Fields | RedCounty 3 |
| Venturas Strip Palms | LasVenturasStrip 30 |
| Young Fir Forest | AngelPineMain 59 |

Want more? → [Harvestables](harvestables.md)
