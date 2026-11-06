namespace OpenRP.Framework.Features.Factions.Enums
{
    /// <summary>
    /// What kind of faction this is. Stored as a number in the database, so never renumber existing values.
    /// </summary>
    public enum FactionType
    {
        /// <summary>
        /// An ordinary faction without a special role.
        /// </summary>
        None = 0,

        /// <summary>
        /// A government. The highest government in a faction tree governs everything below it.
        /// </summary>
        Government = 1,
    }
}
