# Logging

Inject `ILogger<T>` and log with templates. The logs go to the console and to files in `Logs/`, next to the server.

```csharp
_logger.LogError(ex, "Failed to rename Discord channel {ChannelId} to {Name}.", channelId, name);
```

## Where logs go

| Folder | Contents |
|---|---|
| `Logs/General/` | Everything |
| `Logs/ItemManager/` | The item manager |
| `Logs/OpenCDN/` | The CDN |
| `Logs/ServerEvents/` | The server event dispatcher |

Files roll over daily and are kept for 7 days. The Discord library logs here too.

## Known limitations

- **Console only.** Old code that uses `Console.WriteLine` only reaches the console, not the files.

## Deep dive

`AddServerLogging()` (in `Shared/Logging/`) sets up Serilog. To add a folder of your own, copy one of the `WriteTo…Log()` extensions: filter on `Matching.FromSource<T>()`, then call it in `AddServerLogging()`.

Want more? → [Configuration and startup](../getting-started/configuration.md)
