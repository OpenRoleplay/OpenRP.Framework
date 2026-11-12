using OpenRP.Framework.Features.Banking.Enums;

namespace OpenRP.Framework.Features.Banking.Entities
{
    /// <summary>
    /// One character on an account's access list.
    /// </summary>
    public sealed record BankAccessEntry(ulong AccessId, ulong CharacterId, string CharacterName, BankAccessLevel Level);
}
