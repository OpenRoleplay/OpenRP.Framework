# Configuration and startup

The features that need settings, and what the Framework does when the server starts.

## Features with settings

| Call | Options | Page |
|---|---|---|
| `AddActorConversations` | `ChatGptApiKey` | [ActorConversations](../features/actor-conversations.md) |
| `AddActorRoleplay` | `OpenAiApiKey`, `ModelId`, `ReasoningEffort`, `MaxConcurrentReplies` | [ActorRoleplay](../features/actor-roleplay.md) |
| `AddWorldWeather` | weather ids per climate, temperature ranges, map bounds, seed | [WorldWeather](../features/world-weather.md) |
| `AddWorldTime` | in-game days per real day, map bounds, time offset, exempt dates | [WorldTime](../features/world-time.md) |
| `AddCDN` | `BaseUrl`, `Password` | [CDN](../features/cdn.md) |
| `AddDiscord` | bot token, channel ids | [Discord](../features/discord.md) |

## Plugin services

| Service | From | Needed |
|---|---|---|
| `IStreamerService` | `AddStreamer()` from OpenRP.OpenMp.Streamer (our fork of VS-RP's library, with items as ECS components) | Always |
| `IColAndreasService` | `AddColAndreas()` from SampSharp.OpenMp.ColAndreas | Always |
| `ITryg3DService`, `IGpsService` | their own projects | Only if your host uses them |

**SampSharp.OpenMp.ColAndreas** is [our fork](https://github.com/KoertLichtendonk/SampSharp.OpenMp.ColAndreas) of VS-RP's bindings (Apache-2.0), packaged for NuGet as `SampSharp.OpenMp.ColAndreas`. It adds `RayCastLineModel`, which returns the model a ray hits, plus two helpers from ColAndreas's Pawn include: `ColAndreasConstants.WaterObject` (the model id of water, 20000) and `IsPlayerFacingWater`.

**Why a fork:** VS-RP's bindings aren't on NuGet, and fishing needs the model id, which their `RayCastLine` doesn't return.

## What happens at startup

1. **The licence notice** is printed in the console.
2. **Server event handlers** are checked, and mistakes are logged. See [Server events](../server-events.md).
3. **Command permissions:** one `cmd.<name>` permission per command, put in its groups. See [Permissions](../permissions.md).
4. **World data is loaded:** properties, inventories, items, factions, dropped items, actors and vehicles.
5. **Logging** starts. It writes to the console and to `Logs/` (General, ItemManager, OpenCDN, ServerEvents). Files roll daily and are kept for 7 days. See [Logging](../shared/logging.md).

Want more? → [Getting started](../getting-started.md) · [Persistence](../persistence.md)
