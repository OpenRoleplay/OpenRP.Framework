namespace OpenRP.Framework.Features.Banking.Enums
{
    public enum PinCheckResult
    {
        Correct = 1,
        Wrong = 2,
        // Wrong, and that blocked the card; or the card was blocked already.
        Blocked = 3
    }
}
