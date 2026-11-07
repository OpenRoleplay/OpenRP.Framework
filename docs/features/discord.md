# Discord

Connects the game to a Discord server through a bot:
- **Chat bridge:** global OOC and newbie chat go both ways.
- **"Now playing":** a message when someone starts playing.
- **Stats channels:** two channel names show the player count and the in-game date and time.

## How to use it

The host registers it. It must be called even without Discord (empty options), because other systems use it.

| Option | Meaning |
|---|---|
| `DiscordBotToken` | The bot token. Without it, the feature does nothing. Keep it secret. |
| `GeneralChatChannelId` | OOC chat and "now playing" |
| `SupportChannelId` | Newbie chat |
| `PlayerCountChannelId` | Name shows "Players Online: x/y" |
| `ServerDateTimeChannelId` | Name shows "Server Time: dd/MM/yyyy HH:mm" |

## How it works

1. **Game to Discord.** `/ooc` and `/newbie` messages are posted as `**Name:** text`, and `@` is escaped so nobody can be pinged.
2. **Discord to game.** Discord messages from people are shown in game, as `(( Discord OOC | … ))` or `(( Newbie | … [Discord] ))`.
3. **"Now playing"** is posted when a character is selected. Release builds only; **why:** local test servers shouldn't post on the live Discord.
4. **Channel names** go through a queue:
   - each channel is renamed at most once every 5 minutes;
   - the channel waiting longest goes first;
   - the newest name wins.

   **Why:** Discord allows only 2 renames per channel per 10 minutes. Going over makes requests pile up.
5. **The time** is rounded down to the quarter hour. **Why:** it then changes four times per in-game hour, which stays within the limit.

## Known limitations

- **Thread safety.** Discord messages reach players from the Discord library's thread, which breaks the [threading rule](../architecture/threading.md).
- **An extra request.** Every OOC and newbie message first fetches the channel's last message.

Want more? → [How it's built](discord/internals.md) · [RoleplayChats](roleplay-chats.md)
