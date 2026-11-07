# Glossary

Short explanations of the words these docs use.

| Term | Meaning |
|---|---|
| <a id="host"></a>**Host** | The gamemode that uses the Framework, such as OpenRP.Boilerplate or the official server. It adds its own database context, settings and features. |
| <a id="ecs"></a>**ECS** | Entity-component-system: how SampSharp.Entities organises the game. State lives in components on entities; behaviour lives in systems and services. |
| <a id="entity"></a>**Entity** | An identity in the game, such as a player, a door or an inventory item. Only an id (`EntityId`): a type plus a number. |
| <a id="component"></a>**Component** | State attached to an entity. Example: the `Character` component on a player holds their character. |
| <a id="system"></a>**System** | A class whose methods react to events and timers. Example: `PropertySystem` loads properties when the server starts. |
| <a id="service"></a>**Service** | A class with the logic of a feature, given to other code through dependency injection. Example: `ICurrencyService`. |
| <a id="dependency-injection"></a>**Dependency injection** | Code asks for what it needs (for example a service as a method parameter) and the framework hands it over, instead of the code creating it. |
| <a id="singleton"></a>**Singleton / transient** | A singleton is created once and shared. A transient is created new every time it's asked for. Anything holding a database context is transient. |
| <a id="server-event"></a>**Server event** | One of the Framework's own events, such as `OnCharacterSelected`. One feature publishes it, others react. See [Server events](server-events.md). |
| <a id="basemanager"></a>**BaseManager** | The shared class that keeps a table's components in sync with the database. See [Persistence](persistence.md). |
| <a id="seed-data"></a>**Seed data** | Rows every database starts with, such as the permission groups. Written in code with `HasDefaultData`. |
| <a id="migration"></a>**Migration** | A step that changes the database schema (tables and columns), made with Entity Framework. |
| <a id="idempotent-script"></a>**Idempotent script** | A SQL script that is safe to run more than once; it skips what's already done. |
| <a id="cascade"></a>**Cascade / set null** | What happens to a row when the row it points to is deleted: it's deleted too (cascade), or its pointer is cleared (set null). |
| <a id="virtual-world"></a>**Virtual world** | A number that separates players at the same spot. Players only see others in the same virtual world. 0 is the normal world. |
| <a id="interior"></a>**Interior** | GTA's id for an indoor area. 0 is outside. |
| <a id="main-thread"></a>**Main thread** | The one thread the game runs on. Only it may touch players, objects and components. |

Want more? → [Architecture](architecture.md) · [Persistence](persistence.md)
