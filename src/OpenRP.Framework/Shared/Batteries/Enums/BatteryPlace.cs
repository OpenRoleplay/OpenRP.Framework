namespace OpenRP.Framework.Shared.Batteries.Enums
{
    /// <summary>
    /// Where a battery-powered item is, which decides whether its battery drains or recharges.
    /// </summary>
    public enum BatteryPlace
    {
        // In an online character's inventory, also inside a wallet: drains when on, never recharges by itself.
        WithOnlineCharacter = 1,
        // In an offline character's inventory: rests, and recharges once its owner has been away for an hour.
        WithOfflineCharacter = 2,
        // Anywhere else, such as on the ground: drains when on, never recharges by itself.
        Elsewhere = 3
    }
}
