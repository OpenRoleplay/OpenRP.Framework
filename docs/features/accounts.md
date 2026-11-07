# Accounts

Registering and logging in. An account can be logged in only once, on one server, at a time.

## How to use it

There are no commands. Players register and log in from the [main menu](main-menu.md) dialogs.

## How it works

1. **Registering:**
   - a username of 3–24 characters, not taken yet;
   - a password, entered twice.

   The password is stored as a **BCrypt** hash. **Why:** a leaked database doesn't reveal passwords.
2. **Logging in.** The username and password are checked, then the login is **claimed**. If the account is online somewhere else, the login is refused.
3. **After login,** the `Account` component goes on the player, and [`OnAccountLoggedIn`](../server-events.md) is published. The main menu then opens the character selection.

**Why one login per account:** servers share one database, and a character must only be loaded and saved by one server at a time. See [Sessions](../persistence/sessions.md).

## Known limitations

- `Level` and `Experience` on accounts are stored, but not used yet.

## Deep dive

- **Code:** `Features/Accounts/`.
- **Components:** `Account` and `AccountCreation`, both on the player.
- **Services:**
  - `IAccountService`: check, create and log in;
  - `IAccountSessionService`: claim, release and refresh logins;
  - `IAccountDialogService`: the dialogs.
- **System:** `AccountSessionSystem`:
  - releases the login on disconnect;
  - refreshes logins every 30 seconds;
  - releases all logins on shutdown.
- **Table:** [`Accounts`](../database.md#accounts-accountmodel).

Want more? → [Sessions and logins](../persistence/sessions.md) · [Characters](characters.md)
