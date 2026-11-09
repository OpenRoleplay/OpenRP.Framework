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
| `SendActionMessage(name, position, virtualWorld, text)` | `* Name text *` at a spot, like `/me` (for example from an actor) |
| `SendDoMessage(name, position, virtualWorld, text)` | `text (( Name ))` at a spot, like `/do` |
| `SendRoleplayLineToPlayer(player, type, name, text)` | A `/me`, `/do` or talk line by someone else to one player only, for example a conversation they watch from a camera |

**Helpers for AI-written lines** (`Shared/Chat/Helpers/`):

| Helper | Does |
|---|---|
| `RoleplayCommandLine` | Reads a whole `/me`, `/do` or `/say` line, and removes the name in front of `/me` |
| `ChatLineSplitter` | Splits text into parts of at most 110 characters, because the SA-MP client drops long messages |
| `ChatTypingDelay` | How long to pause after a line, as if it was typed |

**Ranges:** `/me`, `/do` and `/my` reach 20 units, `/low` 3 and `/shout` 30, in the sender's virtual world.

**Nearby chat is an event.** Every nearby in-character message from a player (talk, `/me`, `/do`, `/my`, `/low`, `/shout`) also publishes [`OnPlayerChatMessageSent`](../server-events.md#onplayerchatmessagesent), with where it was sent and how far it reached. [AI actors](../features/actor-roleplay.md) use it to hear players.

**Talk** reaches 7 units, in the speaker's virtual world only. From 5 units on it's muffled, with part of the letters turned into dots. Each listener hears their own muffled version. **Why:** far-away speech should be hard to hear, and houses that share an interior must not hear each other.

## Colours

Use the constants, never colour strings:
- `ChatColor` holds codes inside text, such as `ChatColor.Highlight`.
- `ServerColor` holds `Color` values, such as `ServerColor.RoleplayActionColor`.

Want more? → [RoleplayChats](../features/roleplay-chats.md) · [Extensions and colours](extensions.md)
