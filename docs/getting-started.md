# Getting started

To run a server on the Framework, you make a [host](glossary.md#host): a SampSharp gamemode that adds a database context, registers a few services, and calls `AddOpenRoleplayFramework()`. [OpenRP.Boilerplate](https://github.com/OpenRoleplay/OpenRP.Boilerplate) is a working example to copy.

## You need

- open.mp **x64**, with these components in `components/`:
  - SampSharp 1.0;
  - the streamer: `streamer` from [KoertLichtendonk/omp-streamer-component](https://github.com/KoertLichtendonk/omp-streamer-component) plus `OpenRP.OpenMp.Streamer.Native` from [OpenRP.OpenMp.Streamer](https://github.com/OpenRoleplay/OpenRP.OpenMp.Streamer);
  - ColAndreas for open.mp (`colandreas`), with `ColAndreas.cadb`.
- MySQL or MariaDB
- .NET 10
- the `OpenRP.Framework` package

**Why x64:** SampSharp 1.0 runs .NET 10, which only runs 64-bit. 32-bit SA-MP plugins can't load in a 64-bit server, so the plugins are open.mp components.

## Checklist

1. **A database context** that inherits `BaseDataContext` and calls `modelBuilder.ApplyOpenRoleplayFrameworkModelConfigurations()` in `OnModelCreating`.
   - Register it as **transient**, plus `BaseDataContext` as an alias for it.
   - **Why transient:** a [singleton](glossary.md#singleton) context would keep one connection and change tracker for the whole run.
2. **The plugin services:** `AddStreamer()` and `AddColAndreas()`. The Framework uses them, but leaves registering them to the host.
3. **`AddOpenRoleplayFramework()`.**
4. **The six features with settings:** `AddActorConversations`, `AddActorRoleplay`, `AddWorldWeather`, `AddWorldTime`, `AddCDN` and `AddDiscord`.
   - Call them all, even with empty options (`options => { }`).
   - **Why:** other systems depend on their services.
   - **Optional:** `AddEmergencyCalls` for 911. Without it, 911 is not in service.
5. **Your own code:** `AddServerSystemsInAssembly(typeof(Startup).Assembly)`. SampSharp loads the host's own systems itself.
6. **Your own commands:** if you have any, call `ServerCommandCache.RegisterApplicationAssembly(Assembly.GetExecutingAssembly())`.
7. **Startup:** `Startup` implements `IEcsStartup`: `UseEntities().UseCommands()` in `Initialize`, and `EnableStreamerEvents()` in `Configure`. There's no `Program.cs`: SampSharp generates the entry point.
   - **Text encoding:** at the top of `Initialize`, call `Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)` and set `StringViewMarshaller.Encoding = Encoding.GetEncoding(1252)`.
   - **Why:** SA-MP clients show and send chat as Windows-1252, but SampSharp defaults to UTF-8. Without this, anything outside plain ASCII is garbled, such as the curly quotes in AI replies (’ shows as â€™) or a player's "café". For Cyrillic players, use 1251 instead.
8. **Migrations:** make them in the host. See [Seed data and migrations](persistence/seed-data-and-migrations.md).

```csharp
public class Startup : IEcsStartup
{
    public void Initialize(IStartupContext context) => context.UseEntities().UseCommands();

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration) => services
        .AddDbContext<DataContext>(o => o.UseMySql(connectionString, serverVersion), ServiceLifetime.Transient)
        .AddTransient<BaseDataContext>(p => p.GetService<DataContext>())
        .AddStreamer()
        .AddColAndreas()
        .AddOpenRoleplayFramework()
        .AddActorConversations(o => { })
        .AddWorldWeather(o => { })
        .AddWorldTime(o => { })
        .AddCDN(o => { })
        .AddDiscord(o => { })
        .AddServerSystemsInAssembly(typeof(Startup).Assembly);

    public void Configure(IEcsBuilder builder) => builder.EnableStreamerEvents();
}
```

Building needs a .NET 10 SDK new enough for SampSharp's source generator (C# compiler 5.3 or later). With an older SDK the build fails with "no static Main method".

Keep secrets (the Discord token, the CDN password, API keys) out of source control.

## The licence notice

Every player who connects sees the Framework's LGPL notice.

**Why:** the licence requires these credits to stay visible. Don't remove or hide them.

Want more?
- [Options for each feature, and what happens at startup](getting-started/configuration.md)
- [Architecture](architecture.md)
- [Features](README.md#features)
