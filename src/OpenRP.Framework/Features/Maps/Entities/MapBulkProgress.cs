namespace OpenRP.Framework.Features.Maps.Entities
{
    /// <summary>
    /// How far a bulk replace is: the stage it's in, and how many of that stage's items are done out of the total.
    /// </summary>
    public sealed record MapBulkProgress(string Stage, int Done, int Total)
    {
        public const string SavingObjects = "Saving objects";
        public const string SavingTextures = "Saving textures";
        public const string RemovingOldObjects = "Removing old objects";
        public const string AddingObjects = "Adding objects";
        public const string SpawningObjects = "Spawning objects";
    }
}
