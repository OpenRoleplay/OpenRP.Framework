# Emergency calls (911)

Calling 911 from a [phone](phones.md) reaches an AI dispatcher. It asks what's happening, where, and who's involved, and relays the call over the radio to everyone with the `dispatch.receive` permission.

## How to use it

**Callers:** `/call 911`, then talk as in any call.

**Hearing dispatch:** give a faction the permission, for example `/factionpermission 1 dispatch.receive` for the police (see [Permissions](../permissions.md)). Its members then get:
- **in chat:** dispatch's broadcast, for example `[Radio] Dispatch (Police): All units, armed robbery in progress at the 24/7 on Grove Street, Ganton. Suspect is a male in a black jacket with a pistol. No injuries reported. Caller is John Smith, callback 382-0142.`
- **a `/do` from their own position:** "Their radio crackles as dispatch comes through." People near them see it.

**Why a permission:** this stands in for being on the dispatch radio channel until radios exist.

**The host** registers it with an API key (keep the key out of source control):

```csharp
services.AddEmergencyCalls(options =>
{
    options.OpenAiApiKey = configuration["OpenAI_API"];
    options.ModelId = "gpt-6-luna";        // optional
});
```

| Option | Default | What it does |
|---|---|---|
| `OpenAiApiKey` | none | Without it, callers fill in a form instead |
| `ModelId` | `gpt-6-luna` | The OpenAI model |
| `ReasoningEffort` | `none` | `none`, `low`, `medium` or `high`. `none` skips thinking, which is billed as output. Set it to empty for models that don't reason, which reject it |

Without `AddEmergencyCalls`, 911 is "not in service".

## How it works

1. **Answering.** The dispatcher answers "911, what is your emergency?".
2. **Talking.** Everything the caller says goes to the model, with the call so far, the caller ID and the area the call comes in from (such as "Red County"). The model doesn't get the caller's name: it asks, like a real dispatcher.
3. **Replying.** The model writes what it says, and, as soon as it knows what and where, a radio broadcast in its own voice with every detail it knows. New details later in the call lead to an update broadcast.
4. **Hanging up.** The dispatcher hangs up when the call is done or a prank.
5. **Without an API key,** the caller gets a form: Police, Fire or Ambulance, what's happening, and where. The broadcast is built from the answers, the caller's name and number, and the area.

**Limits:**
- **Replies:** at most 8 per call.
  - **Why:** each reply is a paid request.
- **Broadcasts:** at most 3 per call. A new call can't dispatch until a minute after the caller's last dispatch.
  - **Why:** it stops prank floods on the radio.

## Known limitations

- **Coarse area:** the area is the city or county only, so the dispatcher relies on the caller for the exact place.
- **No live dispatcher:** players can't take 911 calls themselves yet.
- **Startup order:** `dispatch.receive` is created at startup. If `/factionpermission` doesn't know it on the very first start, restart the server once.

## Deep dive

- **Code:** `Features/EmergencyCalls/`.
- **911:** `EmergencyNumber` is an `IServerPhoneNumber` (see [Phones](phones.md#deep-dive)). Each call has an `EmergencyCall` component on the call's entity, with what was said.
- **Services:**
  - `IEmergencyDispatcherAi`: the Semantic Kernel call;
  - `IDispatchService`: the radio broadcast and the `/do`s;
  - `IEmergencyCallDialogService`: the form.
- **Helpers:**
  - `EmergencyDispatcherPromptBuilder`: the rules, with the call details last, so OpenAI's prompt caching can reuse the rules;
  - **Caching:** the whole call is sent each turn and only grows, with the cache key `call:{call id}`, so earlier turns are billed at the cached rate. The log shows `AI usage (EmergencyCalls)` for every reply;
  - `EmergencyDispatcherReplyParser`: reads `SAY:`, `DISPATCH:` and `HANGUP` lines and ignores anything else, so stray text never reaches the game. Commands the model puts on one line (`SAY: … DISPATCH: …`) are split first, so a broadcast is never read as speech.
- **`EmergencyDispatchCooldown`:** a component on the caller for the one-minute limit.
- **The area:** [`ZoneNames`](../shared/locations.md).

Want more? → [Phones](phones.md) · [ActorRoleplay](actor-roleplay.md)
