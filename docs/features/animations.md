# Animations

Preloads GTA's animation libraries for every player, when they connect and when they spawn.

**Why:** GTA doesn't play the first animation of a library that isn't loaded yet, so without this, an actor's or player's first animation would fail.

## Deep dive

- **Code:** `Features/Animations/`.
- **Service:** `IAnimationManager.PreloadAnimLibs(player)` applies a "null" animation from every library.
- **System:** `AnimationSystem`, on `OnPlayerConnect` and `OnPlayerSpawn`.

Want more? → [Actors](actors.md)
