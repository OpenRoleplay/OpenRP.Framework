# Actors

Non-player characters standing in the world, loaded from the database, each with a name tag and a looping animation.

## How to use it

There are no commands. Actors are rows in the `Actors` table, seeded by the host.

## How it works

1. **At startup,** every actor is created with its skin, position, animation and name tag. The name comes from the character it's linked to.
2. **AI conversations.** An actor can have an AI prompt, for [conversations](actor-conversations.md).
3. **The main menu.** An actor can belong to a [main menu](main-menu.md) scene.

## Known limitations

- Actors can't be created or edited in-game.

## Deep dive

- **Code:** `Features/Actors/`.
- **Component:** `ServerActor` holds the actor, name tag, id, name and prompt.
- **Service:** `IActorService`: `LoadActors()`, `GetActors()` and `GetServerActors()`.
- **System:** `ActorSystem` loads the actors at `OnGameModeInit`.
- **Tables:** [`Actors`, `ActorPrompts`](../database.md#actors-and-the-main-menu).

Want more? → [ActorConversations](actor-conversations.md) · [MainMenu](main-menu.md)
