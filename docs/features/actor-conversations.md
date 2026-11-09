# ActorConversations

AI-written conversations between [actors](actors.md) that players watch, such as the [main menu](main-menu.md) barbecue. The actors take turns talking and acting in chat as if they were real players. Lines are cached, so watching mostly costs nothing.

For actors that answer players instead, see [ActorRoleplay](actor-roleplay.md).

## How to use it

Every turn is written by ActorRoleplay's generator, so the host registers both. The API key and model are ActorRoleplay's:

```csharp
services
    .AddActorRoleplay(options => options.OpenAiApiKey = configuration["OpenAI_API"])
    .AddActorConversations(options => { });
```

| Option | Default | What it does |
|---|---|---|
| `MaxCachedLines` | 100 | The most lines a conversation gets; then it ends |
| `LinesAhead` | 5 | The next turn is written once a viewer is this close to the end, so nobody waits |

In code (`IActorConversationService`):
1. **Create** it once: `CreateConversation("CONV_FAMILY", actorIds, "A family barbecue on the beach.")`.
2. **Attach** a player: `AttachPlayer("CONV_FAMILY", player)`.
3. **Detach** them again: `DetachPlayer(player)`.

## How it works

1. **One character per turn.** Each request writes only the next speaker's 1 to 3 lines.
   - **What it gets:** that character's prompt, the situation, and the last 25 lines.
   - **How it sees those lines:** the character's own lines as their commands, everyone else's as players see them in chat.
2. **The next speaker** is the character addressed by first name in that turn. Otherwise it's whoever spoke longest ago. A character never speaks twice in a row.
3. **One cache per conversation,** shared by all viewers. Every viewer starts at line 1 and plays at their own pace, with a typing pause after each line.
4. **Writing ahead.** The next turn is only written when a viewer gets within `LinesAhead` lines of the end. Viewers further behind cost nothing.
5. **The ending.** At `MaxCachedLines`, nothing more is written. A viewer who reaches the end is told once that it's time to log in.

**Why one character per turn:** a request only knows the character it writes for. Susan's turn doesn't know Harold's character, so she can't speak for him. One request writing for everyone mixed voices up.

**Why a cache:** most players watch only the first minute while logging in. Replaying cached lines makes that free.

**Why the cap:** it puts a ceiling on what a conversation can cost: about 50 requests for 100 lines, once per restart.

## Known limitations

- **Lost on restart.** The cache is in memory only, so after a restart the first viewers cause new requests again.
- **One cache per server.** Servers sharing a database each build their own.
- **A pause after errors.** After a failed request or a reply without usable lines, a conversation waits 30 seconds before trying again.
- **Names only.** Addressing someone by role ("Dad") doesn't pick the next speaker; the turn order does.

## Deep dive

- **Code:** `Features/ActorConversations/`. Turns are written through ActorRoleplay's `IActorReplyGenerator`, with a `situation`.
- **Components:**
  - `ActorConversation`, on its own entity: the key, the situation, the actors, the cached lines and the next speaker. The actors' names and prompts are looked up at the first turn, because conversations are created at startup, possibly before actors are loaded.
  - `ActorConversationViewer`, on the player: the next line, and when it's due.
- **Service:** `IActorConversationService`: attaching, detaching and `AdvanceViewers()`.
- **System:** `ActorConversationSystem` advances viewers every 250 ms. It only visits viewers, so the cost depends on how many people watch.
- **Helper:** `ActorConversationTurns.ChooseNextSpeaker`.
- **Threading:** everything runs on the main thread. Turns start from the timer and SampSharp brings each `await` back, so there are no locks.

Want more? → [MainMenu](main-menu.md) · [Actors](actors.md) · [ActorRoleplay](actor-roleplay.md)
