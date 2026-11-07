# Sessions and logins

The character a player plays lives on their player entity, and is saved by the Characters feature. An account can be logged in only once, on one server, at a time.

## Session data

| Step | Code |
|---|---|
| Load on selection | `CharacterService.LoadCharacter` puts `Character` and `CharacterPreferences` on the player |
| Save | `CharacterPersistenceSystem`: every 60 seconds and on disconnect |

## One login per account

1. **Claiming.** At login, `AccountSessionService` claims the account with one atomic database update of `IsOnline` and `LastSeenAt`. If it's still online elsewhere, the login is refused.
2. **Refreshing.** Every 30 seconds, each server confirms its logins.
3. **Expiring.** A login not confirmed for **2 minutes** expires.
4. **Releasing.** Logins are released on disconnect and on shutdown.

**Why one login:** several servers can share one database. With one login per account, a character is only loaded and saved by one server at a time, so its data never needs merging.

**Why 2 minutes:** long enough to survive a slow refresh, and short enough that a crashed server doesn't lock accounts for long.

## What this doesn't cover

Shared data, such as faction grants or inventories, can still be changed by another server. Those changes arrive through the [BaseManager](base-manager.md) sync, or at the next load.

Want more? → [Accounts](../features/accounts.md) · [Characters](../features/characters.md)
