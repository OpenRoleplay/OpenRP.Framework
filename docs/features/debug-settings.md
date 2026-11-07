# DebugSettings

Lets testers and admins turn on debug messages from the world clock and weather. The settings aren't saved; they reset when you select a character.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/debug` | Tester, Admin | Turns time, weather or temperature messages on and off |

## How it works

| Setting | Message |
|---|---|
| Time | In-game and local time, every 30 seconds ([WorldTime](world-time.md)) |
| Weather | Weather, wind and temperature, every 5 seconds ([WorldWeather](world-weather.md)) |
| Temperature | Nothing yet; the temperature is part of the weather message |

## Deep dive

- **Code:** `Features/DebugSettingsFeature/`.
- **Component:** `DebugSettings`, on the player.
- **Service:** `IDebugSettingsService`.
- **Loading:** fresh settings go on the player on [`OnCharacterSelected`](../server-events.md).

Want more? → [WorldTime](world-time.md) · [WorldWeather](world-weather.md)
