# Harvestables

Plants players harvest with `/harvest`, such as Indian hemp. Each plant has a label showing how many harvests are left. Gamemodes can add their own plants.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/harvest [resource]` | Default | Harvests the nearest plant of that resource. Without one, lists the resources |

## How it works

1. **Placing.** Plants are placed by the biome generator (`AddHarvestable`) or by code (`CreateHarvestable`). Only registered models become plants.
2. **Starting.** `/harvest hemp` checks that you're near a plant with harvests left, and starts an animation.
3. **Finishing.** After the plant's duration (10 seconds for hemp), the plant loses one harvest, and [`OnResourceHarvested`](../server-events.md#onresourceharvested) is published.

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

## Known limitations

- **No reward by itself.** A gamemode gives the item and experience by handling `OnResourceHarvested`.
- **No regrowth.** Plants don't grow back, and aren't saved.

## Deep dive

- **Code:** `Features/Harvestables/`.
- **The built-in plant:** hemp, with object model 19473.
- **Components:**
  - `HarvestablePlant` (with `IndianHempPlant`);
  - `PlayerHarvesting`, on the player while harvesting.
- **Service:** `IHarvestableService`.
- **System:** `HarvestableSystem` finishes harvests, checking every second.

Want more? → [BiomeGenerator](biome-generator.md)
