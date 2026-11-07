# Getting started

To run a server on the Framework, you make a [host](glossary.md#host): a SampSharp gamemode that adds a database context, registers a few services, and calls `AddOpenRoleplayFramework()`. [OpenRP.Boilerplate](https://github.com/OpenRoleplay/OpenRP.Boilerplate) is a working example to copy.

## You need

- open.mp with the SampSharp, Streamer and ColAndreas plugins
- MySQL or MariaDB
- .NET 6
- the `OpenRP.Framework` NuGet package (each push to its `main` branch publishes a version)

## Checklist

1. **A database context** that inherits `BaseDataContext` and calls `modelBuilder.ApplyOpenRoleplayFrameworkModelConfigurations()` in `OnModelCreating`.
   - Register it as **transient**, plus `BaseDataContext` as an alias for it.
   - **Why transient:** a [singleton](glossary.md#singleton) context would keep one connection and change tracker for the whole run.
2. **The plugin services:** `IStreamerService` and `IColAndreasService`. The Framework uses them, but leaves registering them to the host.
3. **`AddOpenRoleplayFramework()`.**
4. **The five features with settings:** `AddActorConversations`, `AddWorldWeather`, `AddWorldTime`, `AddCDN` and `AddDiscord`.
   - Call them all, even with empty options (`options => { }`).
   - **Why:** other systems depend on their services.
5. **Your own code:** `AddSystemsInAssembly()` and `AddServerSystemsInAssembly()`.
6. **Your own commands:** if you have any, call `ServerCommandCache.RegisterApplicationAssembly(Assembly.GetExecutingAssembly())`.
7. **ECS events:** `EnableSampEvents()`, `EnablePlayerCommands()`, `EnableRconCommands()` and `EnableStreamerEvents()`.
8. **Migrations:** make them in the host. See [Seed data and migrations](persistence/seed-data-and-migrations.md).

```csharp
services
    .AddDbContext<DataContext>(o => o.UseMySql(connectionString, serverVersion), ServiceLifetime.Transient)
    .AddTransient<BaseDataContext>(p => p.GetService<DataContext>())
    .AddSingleton<IStreamerService, StreamerService>()
    .AddSingleton<IColAndreasService, ColAndreasService>()
    .AddOpenRoleplayFramework()
    .AddActorConversations(o => { })
    .AddWorldWeather(o => { })
    .AddWorldTime(o => { })
    .AddCDN(o => { })
    .AddDiscord(o => { })
    .AddSystemsInAssembly()
    .AddServerSystemsInAssembly();
```

Keep secrets (the Discord token, the CDN password, API keys) out of source control.

## The licence notice

Every player who connects sees the Framework's LGPL notice.

**Why:** the licence requires these credits to stay visible. Don't remove or hide them.

Want more?
- [Options for each feature, and what happens at startup](getting-started/configuration.md)
- [Architecture](architecture.md)
- [Features](README.md#features)
