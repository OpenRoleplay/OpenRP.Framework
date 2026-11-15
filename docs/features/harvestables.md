# Harvestables

Plants players harvest with `/harvest`, such as Indian hemp. Each plant has a label showing how many harvests are left. Gamemodes can add their own plants.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/harvest [resource]` | Default | Harvests the nearest plant of that resource. Without one, lists the resources |

## How it works

1. **Placing.** A plant is an object of a harvestable model, such as hemp (model 19473), on a loaded [map](maps.md). When the object spawns, the Framework makes it a plant, with its label. Every map counts: the generated World map and the maps you build with `/editmaps`. Code can also place one with `CreateHarvestable`.
   - **Why:** the plant lives on the object's streamer object, so it goes away when the object despawns.
2. **Starting.** `/harvest hemp` checks that you're near a plant with harvests left, and starts an animation.
3. **Finishing.** After the plant's duration (10 seconds for hemp), the plant loses one harvest, and [`OnResourceHarvested`](../server-events.md#onresourceharvested) is published.
4. **Moving or respawning.** Moving the object in the editor moves the plant and resets its harvests. A respawn does the same, for example when the map loads again, or when the object's model, world or stream distance changes.

## Add your own plant

```csharp
public class SagePlant : HarvestablePlant
{
    public override string GetLabelText() => $"Sage\n{GetRemainingHarvests()} leaves remaining";
}

public class SageHarvestableModel : HarvestableModel<SagePlant>
{
    public override string ResourceName => "sage";
    public override int[] ResourceObjectModelIds => new[] { 12345 };
    public override TimeSpan HarvestDuration => TimeSpan.FromSeconds(8);
}

services.AddHarvestableModel<SageHarvestableModel>();
```

Map objects of its models become plants in the same way, see step 1.

## Known limitations

- **No reward by itself.** A gamemode gives the item and experience by handling `OnResourceHarvested`.
- **No regrowth, and no saving.** A plant's harvests are kept in memory only. A restart, a respawn or an editor move starts them again at full.

## Deep dive

- **Code:** `Features/Harvestables/`.
- **The built-in plant:** hemp, with object model 19473.
- **Components:**
  - `HarvestablePlant` (with `IndianHempPlant`);
  - `PlayerHarvesting`, on the player while harvesting.
- **Service:** `IHarvestableService`.
- **System:** `HarvestableSystem` finishes harvests, checking every second.
- **Map objects:** `HarvestableMapObjectSpawnHandler` uses the [spawn handlers](maps/internals.md#spawn-handlers) of the Maps feature.

Want more? → [Maps](maps.md) · [BiomeGenerator](biome-generator.md)
