# Threading

Only the [main thread](../glossary.md#main-thread) may touch players, objects, natives and components.

**Why:** open.mp, its plugins and SampSharp's entity manager are not thread-safe. Touching them from another thread can crash the server or corrupt state.

## What to use when

| You need to… | Use |
|---|---|
| Wait for the database or the network | `async`/`await`, started on the main thread. The code after `await` returns to the main thread by itself. |
| Do heavy pure computation (for example biome noise) | A worker thread, as long as only its *result* is applied on the main thread |
| Do something later or repeatedly | A `[Timer]` system |
| Anything else on another thread | Don't. No `Task.Run`, `System.Timers` or threads that touch the game. |

## After an await

A player may leave while you wait, and a destroyed component **throws** when used (SampSharp 1.0). After an `await`, check `component.IsComponentAlive` before you use a component you got earlier.

## async void

An `async void` method can't be awaited, so its errors are lost.
- **Server event handlers** must return `Task`.
- **`[Event]` or `[Timer]` handlers** that must stay `async void`: wrap the body in try/catch and log.

## Known exceptions

- **Discord:** messages arriving from Discord are sent to players from a Discord library thread. See [Discord](../features/discord.md).

Want more? → [Architecture](../architecture.md) · [Conventions](conventions.md)
