# Actors

Non-player characters standing in the world, each with a name tag and a looping animation. Admins create and change them in-game with `/editactors`; an actor can stand inside a property, in its own virtual world.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/editactors` | Admin | The actor editor: create, move, rename, skin, animation, virtual world, view prompt, delete |

See [The actor editor](actors/editor.md).

## How it works

1. **Rows in `Actors`.** Each actor is one row: name, skin, position, angle, interior, animation, AI prompt and the property it's inside.
2. **Saving and sync.** Actors are loaded at startup, changes are saved within 60 seconds and on shutdown, and changes made on another server show up after the sync, like [properties](properties.md).
3. **Virtual worlds.** An actor is in its [main menu](main-menu.md) scene's virtual world, the virtual world of the property it's inside, or outside (0).
   - **Why:** an actor in a house's interior should only be seen by visitors of that house, not by everyone using the same GTA interior.
4. **AI prompts.** An actor with a prompt [answers players](actor-roleplay.md) in the world, or takes part in [conversations](actor-conversations.md) in a main menu scene.
5. **Permissions.** An actor can do nothing beyond talking until an admin gives it permissions in `/editactors`, such as a [bank teller's](banking/tellers.md). Features offer them with `services.AddActorPermission(name, description)`, and check them with `IActorPermissionService.HasPermission`.
5. **Staying on the floor.** About 1.5 seconds after an actor streams in for a player, its position and animation are sent again.
   - **Why:** an actor standing on mapping (a porch, an interior floor) can reach a player before the mapping does. The game then drops the actor to the ground below, until its position is sent again.
6. **Deleting a property** moves the actors inside it outside, through the `OnPropertyDeleted` [server event](../server-events.md).

## Known limitations

- **Prompts are read-only in-game.** SA-MP input boxes take about 128 characters and prompts are paragraphs, so they're changed in the database (`Actors.Prompt`).
- **Main menu actors aren't in the editor.** They belong to a scene and its camera; moving them would break it.
- **A brief dip.** A player may briefly see an actor below the floor before it's put back.
- **Old columns.** `ActorCharacterId` and `ActorPromptId` are no longer read, and will be dropped in a later migration.

## Deep dive

- **Code:** `Features/Actors/`.
- **Components:**
  - `ServerActor`: the row's data, with small getters and setters that mark it changed. Its game actor and name label are child entities. `HasActorPrompt()` and `IsInMainMenuScene()` tell where it belongs; `ApplyIdleAnimation()` and `ResetToSpawnPose()` put it back in its pose.
  - `ActorSpawnPoseRefresh` marks an actor whose pose gets sent again, with when. Players streaming it in shortly after each other share one refresh.
- **Services:**
  - `IActorManager`: [BaseManager](../persistence/base-manager.md) persistence, with `CreateAndLoad`. `OnComponentLoaded` (re)creates the game actor and label.
  - `IActorService`: `GetServerActors()`, `GetActorVirtualWorld()`, `CreateActorInWorld()`, `DestroyActorInWorld()` and the spawn-pose refresh.
  - `IActorEditorService`: the editor's logic. Returns an error message, or null.
  - `IActorDialogService`: the editor menus, in `ActorDialogService.cs` and `.Actor.cs`.
- **Systems:**
  - `ActorSystem` loads at `OnGameModeInit`, syncs every 60 seconds, saves on `OnGameModeExit`, schedules a refresh on `OnActorStreamIn`, and checks for due refreshes every 500 ms. The refresh timer only visits the markers, so idle actors cost nothing.
  - `ActorServerSystem` handles `OnPropertyDeleted`.
- **Tables:** [`Actors`](../database.md#actors-and-the-main-menu).

Want more? → [The actor editor](actors/editor.md) · [ActorRoleplay](actor-roleplay.md) · [ActorConversations](actor-conversations.md) · [MainMenu](main-menu.md)
