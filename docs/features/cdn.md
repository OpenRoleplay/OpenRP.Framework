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

The service builds the file's link on the CDN, and plays it with open.mp's audio stream.

**Used by:**
- the [main menu](main-menu.md), for music;
- [chicken coops](chicken-coops.md), for sounds.

## Known limitations

- **No password, no sound.** Without a password the service counts as not configured: it logs an error at startup, and nothing plays.

## Deep dive

- **Code:** `Features/CDN/`.
- **Service:** `IOpenCdnService` has `GetLink`, `Play` and `PlayBase64`, each with and without a position.
- **Logs:** written to `Logs/OpenCDN/`.

Want more? → [Configuration](../getting-started/configuration.md)
