# Adding a setting

A setting is a class that derives from `JurisdictionSettingDefinition<T>`. Register it in its feature's own `Add…()` method, and the settings menus and `IJurisdictionSettingService` find it from there.

**Why:** a new setting is a new key in one table, so adding one never changes the database schema.

## How to add one

1. **Write the class.** The example is a speed limit, modeled on the bank fee cap in `Features/Banking/Entities/MaxMonthlyFeeSetting.cs`.

```csharp
public class SpeedLimitSetting : JurisdictionSettingDefinition<int>
{
    public override string Key => "traffic.speedLimit";

    public override string Name => "Speed limit";

    public override string Prompt => "Enter the speed limit in km/h, from 30 to 130:";

    public override int Default => 50;

    protected override string? TryParse(string input, Jurisdiction jurisdiction, out int value)
    {
        bool isValid = int.TryParse(input, out value) && value is >= 30 and <= 130;
        return isValid ? null : "The speed limit is a whole number from 30 to 130.";
    }

    public override string Format(int value, Jurisdiction jurisdiction)
    {
        return $"{value} km/h";
    }

    // A bound place keeps its own limit, but never above its parent's.
    public override int Bind(int own, int parent)
    {
        return Math.Min(own, parent);
    }

    // Optional: the message when a bound place's value is above its parent's.
    public override string GetBoundError(int parentValue, Jurisdiction parent, Jurisdiction jurisdiction)
    {
        return $"{parent.GetName()} allows at most {Format(parentValue, jurisdiction)}.";
    }

    public override string Serialize(int value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    public override int Deserialize(string storedValue)
    {
        return int.TryParse(storedValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value) ? value : Default;
    }
}
```

2. **Register it** in the feature's own `Add…()` method. `AddBanking()` registers the fee cap with `.AddJurisdictionSetting<MaxMonthlyFeeSetting>()`, and a traffic feature would do the same for the speed limit:

```csharp
public static IServiceCollection AddTraffic(this IServiceCollection self)
{
    return self
        .AddJurisdictionSetting<SpeedLimitSetting>();
}
```

3. **Read the value in effect** with `IJurisdictionSettingService`. Inject the setting the way `BankService` injects `MaxMonthlyFeeSetting`:

```csharp
int speedLimit = _jurisdictionSettingService.GetValue(jurisdiction, _speedLimitSetting);
```

## Rules

- **Never rename the key.** Stored values are found by their key, so a new key would lose them.
- **`Bind` is used for saving and reading.** A value is allowed under a parent when `Bind` leaves it unchanged, so for a limit, return the stricter of the two.
- **`GetBoundError` is optional.** It words the refusal when a bound place's value is above its parent's. Without an override, the text is "{parent} limits this to {value}."
  - **Why:** the default text fits any setting. An override can say what the limit means, such as "allows at most 130 km/h."
- **Stored text is at most 64 characters.** `Deserialize` falls back to the default when the text can't be read.

## Deep dive

- **Code:** `Features/Jurisdictions/Entities/JurisdictionSettingDefinition.cs`, and `AddJurisdictionSetting<TSetting>()` in `Features/Jurisdictions/Extensions/ServiceCollectionExtensions.cs`.
- **Table:** [`JurisdictionSettings`](../../../database.md#jurisdictions). One row holds one setting of one jurisdiction.

Want more? → [Jurisdiction settings](../settings.md) · [Jurisdictions](../../jurisdictions.md) · [Banking](../../banking.md)
