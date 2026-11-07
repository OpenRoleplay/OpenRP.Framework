# WorldTime

An in-game clock that runs faster than real time: by default three in-game days per real day. The time also shifts a little across the map.

**Why:**
- **Three days a day:** players in every time zone get to see day and night.
- **The date:** the year is 33 years back, which puts it in GTA San Andreas' era (the 1990s).
- **The shift:** it makes the map feel large, like real time zones.

## How to use it

The host registers it, even with empty options:

| Option | Default | Meaning |
|---|---|---|
| `IngameCyclesPerRealDay` | 3 | In-game days per real day |
| `TotalPositionOffsetInSeconds` | 900 | Shift across the map, east to west (±15 min) |
| `MapXMin`, `MapXMax` | -3000, 3000 | The map's width |
| `ExemptDates` | none | Dates when in-game time equals real UTC time |

## How it works

Every 30 seconds:
- **The clock.** Each player's clock is set to the local time at their spot (further west is later). Players with a `/setweathertime` override are skipped.
- **Debug.** Time debug messages go out ([DebugSettings](debug-settings.md)).
- **Discord.** The Discord date/time channel is updated ([Discord](discord.md)).

## Deep dive

- **Code:** `Features/WorldTime/`.
- **Service:** `IWorldTimeService`: `GetCurrentIngameDateTime()`, `GetCurrentIngameTime()` and `GetCurrentSimulationTime(x)`.

Want more? → [WorldWeather](world-weather.md)
