# ActorConversations

AI-driven conversations between [actors](actors.md), written live by OpenAI's ChatGPT. Players nearby see the actors talk and act, as if they were real players.

For actors that answer players instead, see [ActorRoleplay](actor-roleplay.md).

## How to use it

The host registers it with an API key (keep the key out of source control):

```csharp
services.AddActorConversations(options => options.ChatGptApiKey = configuration["ChatGptApiKey"]);
```

In code:
1. Create a conversation: `CreateConversation("CONV_FAMILY", actorIds)`.
2. Attach a player: `AttachPlayerToConversationAsync(name, player)`.
3. Detach them again: `DetachPlayerFromConversationAsync(name, player)`.

## How it works

1. **Prompts.** Each actor's prompt describes who they are.
2. **Generating.** While players are attached, the next lines are generated from the actors, the date and the history so far.
3. **Playing.** The reply is turned into speech, actions and typing delays.

Lines are only generated while players are attached. Each exchange is a paid OpenAI request.

## Known limitations

- Without a valid API key, nothing is generated.

## Deep dive

- **Code:** `Features/ActorConversations/`. It uses the `ChatGPT.Net` package.
- **Service:** `IActorConversationWithPlayerManager`.
- **Helpers:**
  - `ActorConversationWithPlayerHelper` runs a conversation;
  - the prompt builder writes the prompt;
  - `ActorCommandParser` and `ActorCommandProcessor` turn replies into chat messages.
- **Component:** `ActorConversationWithPlayerState`, on the player: the conversation and its sequence.

Want more? → [MainMenu](main-menu.md) · [Actors](actors.md)
