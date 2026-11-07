# Admins

Admin and tester commands that don't belong to another feature. Other admin tools live with their feature: `/createfaction` in [Factions](factions.md), `/editproperties` in [Properties](properties.md), `/createvehicle` in [Vehicles](vehicles.md).

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/givemoney [receiver] [currency code] [amount]` | Admin | Gives money; a negative amount takes it |
| `/givepermissiongroup [player] [group]` | Admin | Gives a [permission group](../permissions.md), effective at once |
| `/gotopos [x] [y] [z] <interior> <virtual world>` | Admin | Teleports, for example into a GTA interior. Accepts `.` or `,` as the decimal separator |
| `/setweathertime [weather] [hour] [minutes]` | Admin | Sets your own weather and time; the world systems then skip you |
| `/sv`, `/spawnveh`, `/spawnvehicle [model] <color> <color>` | Admin, Tester | Spawns a temporary vehicle and puts you in it |
| `/testerkit` | Tester, Admin | Gives you the hotwire tools |

## Known limitations

- **No moderation tools yet:** no kick, ban, mute, spectate, reports or admin chat.
- **Temporary vehicles.** `/sv` vehicles aren't saved and have no key.
- **No `/help` group.** `/setweathertime` and `/testerkit` appear ungrouped in `/help`.

## Deep dive

- **Code:** `Features/Admins/`.
- **Component:** `WeatherTimeOverride` marks a player whose weather and time were set by hand.

Want more? → [Commands](../commands.md) · [Permissions](../permissions.md)
