# Emergency calls (911)

Calling 911 from a [phone](phones.md) reaches an AI dispatcher. It asks what's happening, where, and who's involved, and relays the call over the [radio](radios.md) on frequency 911.

## How to use it

**Callers:** `/call 911`, then talk as in any call.

**Hearing dispatch:** carry a [radio](radios.md) tuned to 911 (`/giveradio`, then `/freq 911`), or sit in a vehicle whose radio is. Police, fire and EMS all share this one frequency for now.
- **On the radio:** dispatch's broadcast, for example `[Radio 911 (911 Dispatch)] Dispatch (Police): All units, armed robbery in progress at the 24/7 on Grove Street, Ganton. Suspect is a male in a black jacket with a pistol. No injuries reported. Caller is John Smith, callback 382-0142.`
- **Around the radio:** people near a radio that plays out loud overhear it, unless its holder uses an earpiece.

**The frequency** is created at startup as "911 Dispatch", protected and without factions: anyone with a radio can listen in, as with real 1992 scanners, but nobody can talk on it. Add the factions that may talk with [`/editfrequencies`](radios/frequencies.md).

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
| `DispatchFrequency` | `911` | The radio frequency dispatch broadcasts on |

Without `AddEmergencyCalls`, 911 is "not in service".

## How it works

1. **Answering.** The dispatcher answers "911, what is your emergency?".
2. **Talking.** The dispatcher waits until the caller has been quiet for 5 seconds, so several lines in a row get one answer, like with [AI actors](actor-roleplay.md). Everything the caller says goes to the model, with the call so far, the caller ID and the area the call comes in from (such as "Red County"). The model doesn't get the caller's name: it asks, like a real dispatcher.
3. **Replying.** The model writes what it says, and, as soon as it knows what and where, a radio broadcast in its own voice with every detail it knows. New details later in the call lead to an update broadcast.
4. **Hanging up.** Once help is dispatched and the caller has nothing new (an "okay" or "thanks"), the dispatcher says goodbye and hangs up in the same reply. It also hangs up on pranks.
5. **Without an API key,** the caller gets a form: Police, Fire or Ambulance, what's happening, and where. The broadcast is built from the answers, the caller's name and number, and the area.

**Limits:**
- **Replies:** at most 8 per call.
  - **Why:** each reply is a paid request.
- **Broadcasts:** at most 3 per call. A new call can't dispatch until a minute after the caller's last dispatch.
  - **Why:** it stops prank floods on the radio.

## Known limitations

- **Coarse area:** the area is the city or county only, so the dispatcher relies on the caller for the exact place.
- **No live dispatcher:** players can't take 911 calls themselves yet.
- **One frequency for every service:** police, fire and EMS all hear each other's calls.
- **Old grants:** the `dispatch.receive` permission from before radios does nothing anymore. It stays in the database until an admin removes it.

## Deep dive

- **Code:** `Features/EmergencyCalls/`.
- **911:** `EmergencyNumber` is an `IServerPhoneNumber` (see [Phones](phones.md#deep-dive)). Each call has an `EmergencyCall` component on the call's entity, with what was said.
- **Services:**
  - `IEmergencyDispatcherAi`: the Semantic Kernel call;
  - `IDispatchService`: the broadcast, sent with `IRadioBroadcastService.TransmitFromSystem`, and creating the frequency on [`OnRadiosLoaded`](../server-events.md#onradiosloaded);
  - `IEmergencyCallDialogService`: the form.
- **Helpers:**
  - `EmergencyDispatcherPromptBuilder`: the rules, with the call details last, so OpenAI's prompt caching can reuse the rules;
  - **Caching:** the whole call is sent each turn and only grows, with the cache key `call:{call id}`, so earlier turns are billed at the cached rate. The log shows `AI usage (EmergencyCalls)` for every reply;
  - `EmergencyDispatcherReplyParser`: reads `SAY:`, `DISPATCH:` and `HANGUP` lines and ignores anything else, so stray text never reaches the game. Commands the model puts on one line (`SAY: … DISPATCH: …`) are split first, so a broadcast is never read as speech. A hang-up counts in any case and decoration (`HANGUP.`, `[HANGUP]`, `Hang up`), as long as it's on its own line. Each raw reply is logged at Debug level.
- **`EmergencyDispatchCooldown`:** a component on the caller for the one-minute limit.
- **The area:** [`ZoneNames`](../shared/locations.md).

Want more? → [Phones](phones.md) · [Radios](radios.md) · [ActorRoleplay](actor-roleplay.md)
