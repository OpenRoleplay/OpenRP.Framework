# Chat

`Shared/Chat/` sends system messages and roleplay chat. The chat commands themselves are in [RoleplayChats](../features/roleplay-chats.md).

## Messages to one player

```csharp
player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You are not near any door!");
```

| Type | Shows as |
|---|---|
| `INFO` | `[INFO]` in blue |
| `ERROR` | `[ERROR]` in red |
| `SYNTAX` | `[SYNTAX]` in orange, for command syntax |
| `DEBUG` | `[DEBUG]` in light blue |
| `ADMIN` | `[ADMIN]` in dark blue |

## IChatService

| Method | Does |
|---|---|
| `SendInfoMessage(type, text)` | Info message to everyone |
| `SendTalkMessage(player, text)` | The character says something to people nearby |
| `SendTalkMessage(name, position, text, accent, virtualWorld)` | Someone (for example an actor) talks at a spot. `virtualWorld` defaults to 0 |
| `SendPlayerChatMessage(player, type, text)` | A roleplay message (`ME`, `DO`, `LOW`, `SHOUT`, `OOC`, …) |

**Talk** reaches 7 units, in the speaker's virtual world only. From 5 units on it's muffled, with part of the letters turned into dots. Each listener hears their own muffled version. **Why:** far-away speech should be hard to hear, and houses that share an interior must not hear each other.

## Colours

Use the constants, never colour strings:
- `ChatColor` holds codes inside text, such as `ChatColor.Highlight`.
- `ServerColor` holds `Color` values, such as `ServerColor.RoleplayActionColor`.

Want more? → [RoleplayChats](../features/roleplay-chats.md) · [Extensions and colours](extensions.md)
