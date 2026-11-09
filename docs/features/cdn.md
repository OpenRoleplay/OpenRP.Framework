# CDN

Plays audio files from an OpenCDN server to players: for the player only, or from a spot in the world within a range.

## How to use it

The host registers it (keep the password out of source control):

```csharp
services.AddCDN(options =>
{
    options.BaseUrl = configuration["Cdn:BaseUrl"];
    options.Password = configuration["Cdn:Password"];
});
```

In code: `Play(player, subDir, path)`, or `Play(player, subDir, path, position, range)` to play from a spot.

## How it works

1. The service builds a link to the file that is signed with the password and expires after 10 minutes.
2. open.mp's audio stream sends the link to the player's game, which downloads and plays the file.
3. The CDN checks the signature and the expiry before it sends the file.

**Why:** players can see the link, so it only works for that one file and only for a short time. That keeps the files to players on the server instead of anyone with a copied link.

**Used by:**
- the [main menu](main-menu.md), for music;
- [chicken coops](chicken-coops.md), for sounds.

## Known limitations

- **No BaseUrl or password, no sound.** The service counts as not configured: it logs an error at startup, and nothing plays.
- **A copied link still works** for that one file until it expires, 10 minutes after it was made.
- **Players can record** whatever they hear; the links only keep others out.

## Deep dive

- **Code:** `Features/CDN/`.
- **Service:** `IOpenCdnService` has `GetLink`, `Play` and `PlayBase64`, each with and without a position.
- **Logs:** written to `Logs/OpenCDN/`.

### The link format

```
{BaseUrl}/{expires}/{signature}/{subDir}/{file}
```

| Part | Value |
|---|---|
| `expires` | Unix time in seconds (UTC) when the link stops working: now plus 10 minutes |
| `signature` | HMAC-SHA256 with the password as key, over `{expires}/{subDir}/{file}`, in lowercase hex |
| `file` | The file path, URL-escaped in the link; the signature covers the unescaped path |

A CDN for this service must:
1. reject the request when `expires` has passed;
2. compute the same HMAC over `{expires}/{subDir}/{file}` and reject the request when it doesn't match (compare in constant time);
3. only serve files inside its own folder.

**Why Unix time:** it's the same on the game server and the CDN, whatever their time zones.

Want more? → [Configuration](../getting-started/configuration.md)
