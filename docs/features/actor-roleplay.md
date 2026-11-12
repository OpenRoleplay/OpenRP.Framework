# ActorRoleplay

AI [actors](actors.md) that roleplay back. When a player uses `/me`, `/do` or `/say` near one, the actor answers in character with up to 3 lines of `/me`, `/do` or `/say`, written by an OpenAI model through Semantic Kernel.

## How to use it

The host registers it with an API key (keep the key out of source control):

```csharp
services.AddActorRoleplay(options =>
{
    options.OpenAiApiKey = configuration["OpenAI_API"];
    options.ModelId = "gpt-6-luna";        // optional
});
```

| Option | Default | What it does |
|---|---|---|
| `OpenAiApiKey` | none | Without it, actors don't reply |
| `ModelId` | `gpt-6-luna` | The OpenAI model |
| `ReasoningEffort` | `none` | `none`, `low`, `medium` or `high`. `none` skips thinking, which is billed as output. Set it to empty for models that don't reason, which reject it |
| `MaxConcurrentReplies` | 4 | OpenAI requests at once, across all actors |

**Making an actor an AI actor:** give it a prompt (who they are, how they talk) in `Actors.Prompt`. Actors in a [main menu](main-menu.md) scene never reply.

## How it works

1. **Hearing.** Every `/me`, `/do` and `/say` publishes [`OnPlayerChatMessageSent`](../server-events.md). AI actors within the message's range (20 m for `/me` and `/do`, 7 m for `/say`) in the same virtual world remember the line as it appeared in chat.
2. **Waiting.** The actor waits until it has heard nothing for 5 seconds, so a `/me` and a `/say` right after each other get one reply. SA-MP doesn't tell the server when someone is typing, so this pause is what gives players time for a follow-up line. The 911 dispatcher waits the same way (`Shared/AI/Helpers/AiReplyTiming`).
3. **Replying.**
   - The model gets the shared roleplay rules, the actor's prompt, and up to 40 lines it heard and said. Past 40, the oldest 20 go in one go.
   - Its reply is split into `/me`, `/do` and `/say` lines and sent from the actor's position, with a short typing pause between lines.
   - Lines heard during a reply lead to the next reply.
4. **Forgetting.** An actor that heard nothing for 15 minutes forgets the scene, so the next visitor starts fresh.

## Actions

An AI actor can also do things for the player it talks with, such as a bank teller opening the deposit form. It writes one extra reply line:

```
/action bank_deposit Kir Polzin 500
```

1. **Only what it's allowed:** the actor's prompt lists only the actions it has the [actor permission](actors.md) for and can do where it stands. With none, nothing is added to the prompt.
2. **Only for someone it heard:** the name is matched against the players who spoke to the actor in the last 5 minutes and still stand within 10 units, in the same virtual world. With only one such player, that one. Anyone else is never reached, whatever name the model writes.
3. **The detail** after the name (here 500) is free text that the action checks like player input.
4. **The action checks again** before doing anything, and remembers the line, so the actor knows it did it.
- **Why a reply line, not Semantic Kernel tool calls:** one request instead of two, and the action runs on the main thread, where dialogs must be opened.
- **Adding one:** implement `IActorAction` and register it with `services.AddActorAction(provider => new MyAction(...))`. See [Banking](banking.md) for the teller's actions.

**Why wait for a pause:** players often send an action and speech right after each other. Answering each separately gives double replies and costs twice as much.

**Why actors don't hear each other's AI lines:** two actors would otherwise keep a paid conversation going between themselves.

## Many actors

- **Idle actors cost nothing.** Hearing is one distance check per actor per message; memory is only added once an actor hears something.
- **Request slots.** At most `MaxConcurrentReplies` requests run at once; other actors wait their turn.
  - **Why:** it stays under OpenAI's rate limits when many players talk to many actors.
- **One client.** All actors share one Semantic Kernel chat service.
- **Prompt caching.** OpenAI bills input it has seen before at a tenth of the price (from 1,024 tokens).
  - **Stable starts:** the shared rules come first, then the actor's prompt, then the memory. Memory is only trimmed in chunks of 20, so each request starts the same way as the one before and only the newest lines are new.
  - **Cache keys:** each actor has one, `actor:{id}`, so its requests reach the same cache.
  - **Checking it:** the log shows `AI usage (ActorRoleplay): 1534 in (1280 cached), 64 out` for every reply.
  - **Why trim in chunks:** dropping one old line per reply changes the start of every request, so nothing after the rules would ever be cached.
- **Why GPT-6 Luna:** $0.10 per million tokens in, $0.01 cached and $0.50 out (November 2026), with thinking turned off. That's about 8 times cheaper than `gpt-5.4-mini`, and it writes short roleplay lines well.

## Known limitations

- **Paid requests.** Each reply is a paid OpenAI request.
- **Memory doesn't last.** It's lost on a restart, and shared by everyone near the actor.
- **Few actions.** Actors can open forms through [actions](#actions), such as a bank teller's, but can't hand over items or money themselves.
- **No moving or animating.** A reply is only chat; the actor doesn't move or animate.

## Deep dive

- **Code:** `Features/ActorRoleplay/`. It uses the `Microsoft.SemanticKernel` package.
- **Services:**
  - `IActorRoleplayService`: `HearPlayerMessage`, `ReplyToQuietActors` and `ForgetIdleActors`;
  - `IActorReplyGenerator`: the Semantic Kernel call. It writes one character's next 1 to 3 lines and returns them parsed. An optional `situation` adds where the character is and what's expected of them; [ActorConversations](actor-conversations.md) uses it to write each scene turn, so both features share one engine and one set of settings.
- **Components on the actor's entity:**
  - `ActorRoleplayMemory`: the lines, the players who spoke lately (for actions), and whether it's replying;
  - the `ActorAwaitingReply` marker: the reply timer only looks at these.
- **Systems:**
  - `ActorRoleplayServerSystem`: hears the event;
  - `ActorRoleplaySystem`: a 500 ms reply timer and a 1 minute forgetting timer.
- **Helpers:**
  - `ActorRoleplayPromptBuilder`: the system prompt;
  - `ActorReplyParser`: only whole lines starting with a command count, so "24/7" in speech survives; `/action` lines are read separately, at most one per reply;
  - shared, in `Shared/Chat/Helpers/`: `RoleplayCommandLine` reads `/me`, `/do` and `/say` lines; `ChatLineSplitter` makes parts of at most 110 characters, because the SA-MP client drops long messages; `ChatTypingDelay` sets the pauses.
  - the Semantic Kernel chat service comes from `Shared/AI/Helpers/OpenAiChatCompletion`. It sends the reasoning effort and the cache key as raw request fields, because Semantic Kernel refuses efforts it doesn't know, such as `none`, before sending.
- **Threading:** replies start from the main-thread timer, and SampSharp brings every `await` back to the main thread. After each `await`, the code checks the actor still exists.

Want more? → [Actors](actors.md) · [ActorConversations](actor-conversations.md)
