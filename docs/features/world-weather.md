# WorldWeather

Weather that differs across the map and changes over time. The map has temperate, humid and arid climates, each with its own weather types and temperatures.

**Why:** a living world. The desert should be dry and the coast foggy, rather than one weather for everyone.

## How to use it

The host registers it, even with empty options:

| Option | Default | Meaning |
|---|---|---|
| `TemperateWeatherIds` | 0–9, 13–16 | GTA weather ids for temperate areas |
| `HumidWeatherIds` | 5–9, 13–16 | GTA weather ids for humid areas |
| `AridWeatherIds` | 10–12, 17–19 | GTA weather ids for arid areas |
| `MinTemperature…` / `MaxTemperature…` | Humid 0–20, Arid 25–40, Temperate 10–25 | °C |
| `MapXMin/XMax/YMin/YMax` | ±3000 | The map's size |
| `Seed` | random | Set it for weather that repeats after a restart |

## How it works

Every 5 seconds:
- **Weather.** Each player gets the weather at their spot. Players with a `/setweathertime` override are skipped.
- **Debug.** Weather debug messages (weather, wind, temperature) go to those who turned them on.

The weather comes from noise (`Shared/FastNoiseLite`). The wind has a direction that changes over time.

## Known limitations

- **No effect from wind or temperature yet.** The wind doesn't move the weather (`GetWindOffset` is unused).
- **The temperature debug setting does nothing.**

## Deep dive

- **Code:** `Features/WorldWeather/`.
- **Services:**
  - `IWorldWeatherService`: `GetWeatherAt`, `PredictWeather`, `GetWindSpeedAt`, `GetTemperatureAt`, `IsPositionHumid` and `IsPositionArid`;
  - `IWorldWindDirectionService`.

Want more? → [WorldTime](world-time.md) · [DebugSettings](debug-settings.md)
