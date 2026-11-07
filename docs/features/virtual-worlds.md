# VirtualWorlds

Hands out [virtual world](../glossary.md#virtual-world) numbers, so features that need a world of their own don't clash. Code: `Features/VirtualWorlds/`.

## How it works

The first time a key asks, it gets the next free number, from 1 up. After that, the same key gets the same number, until the server restarts.

| Method | Used by |
|---|---|
| `GetMainMenuSceneVirtualWorld(sceneId)` | [MainMenu](main-menu.md) scenes |
| `GetPropertyVirtualWorld(propertyId)` | [Properties](properties.md): doors inside |

World 0 is the normal world, and is never handed out.

## Known limitations

- **Numbers aren't saved,** so they can change after a restart. Don't store them; ask again.

Want more? → [Properties](properties.md)
