# Discord: internals

How the Discord feature is built (`Features/Discord/`). For what it does, see [Discord](../discord.md).

## Library

DSharpPlus 4.5. It gets the server's logger factory, so its warnings, rate limits included, go to the server logs.

## Services

**`IDiscordService`**

| Method | Does |
|---|---|
| `SendGeneralChatMessage(text)` | Posts in the general channel |
| `SendGlobalOocChatMessage(player, text)` | Adds a `## [In-Game OOC]` header when the last message isn't the bot's |
| `SendNewbieChatMessage(player, text)` | The same, with `## [In-Game Newbie Chat]` |
| `UpdatePlayerCount(decrease)` | Queues the player count name |
| `UpdateDateTime(date, time)` | Queues the date and time name |
| `SendNextChannelRenameAsync()` | Renames the channel waiting longest, when allowed; logs errors |

**`IDiscordChannelNameQueue`**

| Method | Does |
|---|---|
| `SetChannelName(id, name, now)` | Queues a name. A newer name replaces a waiting one, and keeps its place |
| `TakeNextRename(now)` | The rename that may go now, or null. One at a time, with a 5-minute cooldown per channel |
| `MarkRenamed(rename, renamedAt)` | Records it. `renamedAt` is null when the channel already had the name |
| `MarkRenameFailed(rename)` | Puts it back in line at its old place |

The queue takes the time as a parameter, so it can be tested without Discord. `Helpers/DiscordChannelNames` builds the names using the invariant culture. **Why:** the server's Dutch culture would turn the date's slashes into dashes.

## Systems

| System | Does |
|---|---|
| `DiscordSystem` | Player count on startup, connect and disconnect |
| `DiscordServerSystem` | "Now playing" on [`OnCharacterSelected`](../../server-events.md) |
| `DiscordQueueSystem` | Checks the rename queue every 15 seconds |

[WorldTime](../world-time.md) calls `UpdateDateTime` every 30 seconds.

Want more? → [Discord](../discord.md) · [Logging](../../shared/logging.md)
