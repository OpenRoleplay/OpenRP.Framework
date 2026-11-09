# Actors

Non-player characters standing in the world, loaded from the database, each with a name tag and a looping animation.

## How to use it

There are no commands. Actors are rows in the `Actors` table, seeded by the host.

## How it works

1. **At startup,** every actor is created with its skin, position, animation and name tag. The name comes from the character it's linked to.
2. **AI prompts.** An actor can have an AI prompt. In the world it then [answers players](actor-roleplay.md); in a main menu scene it takes part in [conversations](actor-conversations.md).
3. **The main menu.** An actor can belong to a [main menu](main-menu.md) scene.
4. **Staying on the floor.** About 1.5 seconds after an actor streams in for a player, its position and animation are sent again.

**Why:** an actor standing on mapping (a porch, an interior floor) can reach a player before the mapping does. The game then drops the actor to the ground below, until its position is sent again.

## Known limitations

- Actors can't be created or edited in-game.
- A player may briefly see an actor below the floor before it's put back.

## Deep dive

- **Code:** `Features/Actors/`.
- **Components:**
  - `ServerActor` holds the actor, name tag, id, name and prompt. `HasActorPrompt()` and `IsInMainMenuScene()` tell where the actor belongs; `ApplyIdleAnimation()` and `ResetToSpawnPose()` put it back in its pose.
  - `ActorSpawnPoseRefresh` marks an actor whose pose gets sent again, with when. Players streaming it in shortly after each other share one refresh.
- **Service:** `IActorService`: `LoadActors()`, `GetActors()`, `GetServerActors()`, `ScheduleSpawnPoseRefresh(actor)` and `RefreshDueSpawnPoses()`.
- **System:** `ActorSystem` loads the actors at `OnGameModeInit`, schedules a refresh on `OnActorStreamIn`, and checks for due refreshes every 500 ms. The timer only visits the markers, so idle actors cost nothing.
- **Tables:** [`Actors`, `ActorPrompts`](../database.md#actors-and-the-main-menu).

Want more? → [ActorRoleplay](actor-roleplay.md) · [ActorConversations](actor-conversations.md) · [MainMenu](main-menu.md)
